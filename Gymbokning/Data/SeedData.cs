using Bogus;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace Gymbokning.Data
{
    public class SeedData
    {
        private static ApplicationDbContext _context = default!;
        private static UserManager<ApplicationUser> userManager = default!;
        private static RoleManager<IdentityRole> roleManager = default!;

        public static async Task InitAsync(ApplicationDbContext context, IServiceProvider services, string adminPSW)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));
            _context = context;

            ArgumentNullException.ThrowIfNull(nameof(services));

            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            ArgumentNullException.ThrowIfNull(nameof(userManager));

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            ArgumentNullException.ThrowIfNull(nameof(roleManager));

            if (_context.GymClasses.Where(g => g.StartTime > DateTime.Now).Any()) return;

            var gymClasses = GetFakeGymClasses(10);
            await _context.AddRangeAsync(gymClasses);
            await _context.SaveChangesAsync();

            var roleNames = new[] { "Member", "Admin" };        // Defeniera roller
            await CreateRolesAsync(roleNames);

            var adminEmail = "admin@Gymbokning.se";
            var adminFirstName = "Lazy";
            var adminLastName = "Lars";
            var admin = await CreateUserAsync(adminEmail, adminPSW, adminFirstName, adminLastName);

            await AddAdminToAllRolesAsync(admin, roleNames);

        }

        private static IEnumerable<GymClass> GetFakeGymClasses(int quantity)
        {
            var faker = new Faker("sv");
            var gymClases = new List<GymClass>();
            Random rnd = new Random();

            for (int i = 0; i < quantity; i++)
            {
                var gymClass = new GymClass
                {
                    Name = faker.Hacker.Adjective(),
                    StartTime = DateTime.Now.AddDays(rnd.Next(-5, 6)),
                    Duration = new TimeSpan(0, rnd.Next(40, 60), 0),
                    Description = faker.Company.Bs()
                };

                gymClases.Add(gymClass);
            }

            return gymClases;
        }

        private static async Task CreateRolesAsync(string[] roleNames)
        {
            foreach(var roleName in roleNames)
            {
                if(await roleManager.RoleExistsAsync(roleName)) continue;

                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if(!result.Succeeded) throw new Exception($"\n{result.Errors}");
            }

        }

        private static async Task<ApplicationUser> CreateUserAsync(string email, string psw, string? FirstName, string? LastName)
        {
            var found = await userManager.FindByEmailAsync(email);
            if(found != null) return null!;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = FirstName,
                LastName = LastName
            };

            var result = await userManager.CreateAsync(user, psw);
            if(!result.Succeeded) throw new Exception($"\n{result.Errors}");

            return user;
        }

        private static async Task AddAdminToAllRolesAsync(ApplicationUser admin, string[] roleNames)
        {
            foreach(string role in roleNames)
            {
                if(await userManager.IsInRoleAsync(admin, role)) continue;

                var result = await userManager.AddToRoleAsync(admin, role);
                if (!result.Succeeded) throw new Exception($"\n{result.Errors}");
            }
        }

    }
}
