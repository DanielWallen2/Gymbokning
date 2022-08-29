using Gymbokning.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models
{
    public class GymClass
    {
        public int Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Starttid")]
        [Remote("ValidateStartTime", "GymClasses")]
        [ValidateStarttimePassed(ErrorMessage = "This time has allready passed")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Längd")]
        public TimeSpan Duration { get; set; }

        [Display(Name = "Sluttid")]
        public DateTime EndTime { get { return (StartTime + Duration); } }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        // Navigational properties
        public ICollection<ApplicationUserGymClass> GymClassMembers { get; set; } = new List<ApplicationUserGymClass>();

    }
}
