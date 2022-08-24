using Bogus;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;

namespace Gymbokning.Data
{
    public class SeedData
    {
        private static ApplicationDbContext _context = default!;
        private static UserManager<ApplicationUser> userManager = default!;
        private static RoleManager<IdentityRole> roleManager = default!;

        public static async Task InitAsync(ApplicationDbContext context, IServiceProvider services, string adminPW)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));
            _context = context;

            ArgumentNullException.ThrowIfNull(nameof(services));

            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            ArgumentNullException.ThrowIfNull(nameof(userManager));

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            ArgumentNullException.ThrowIfNull(nameof(roleManager));

            var roleNames = new[] { "Member", "Admin" };        // Defeniera roller
            var adminEmail = "admin@Gymbokning.se";

            var gymClasses = GetFakeGymClasses(10);
            await _context.AddRangeAsync(gymClasses);


        }

        private static object GetFakeGymClasses(int quantity)
        {
            var faker = new Faker("sv");
            var gymClases = new List<GymClass>();
            Random rnd = new Random();

            for (int i = 0; i < quantity; i++)
            {
                var gymClass = new GymClass
                {
                    Name = faker.Company.Bs(),
                    StartTime = DateTime.Now.AddDays(rnd.Next(-5, 11)),
                    Duration = new TimeSpan(0, rnd.Next(40, 21), 0),
                    Description = faker.Hacker.Adjective()
                };

                gymClases.Add(gymClass);
            }

            return gymClases;
        }
    }
}
