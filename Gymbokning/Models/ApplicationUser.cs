using Microsoft.AspNetCore.Identity;

namespace Gymbokning.Models
{
    public class ApplicationUser : IdentityUser
    {
        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string Password { get; set; }
        //public string Email { get; set; }

        [PersonalData]
        public DateTime MembershipStarted { get; set; } = DateTime.Now;

        // Navigational properties
        public ICollection<ApplicationUserGymClass> UserGymClasses { get; set; }
    }
}
