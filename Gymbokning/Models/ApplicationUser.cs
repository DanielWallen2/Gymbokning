using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "Förnamn")]
        public string? FirstName { get; set; }

        [Display(Name = "Efternamn")]
        [PersonalData]
        public string? LastName { get; set; }

        [Display(Name = "Namn")]
        [PersonalData]
        public string FullName => $"{FirstName} {LastName}";

        [PersonalData]
        public DateTime MembershipStarted { get; set; } = DateTime.Now;

        // Navigational properties
        public ICollection<ApplicationUserGymClass> UserGymClasses { get; set; }
    }
}
