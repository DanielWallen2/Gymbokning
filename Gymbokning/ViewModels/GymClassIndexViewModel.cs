using Gymbokning.Models;
using System.ComponentModel.DataAnnotations;

namespace Gymbokning.ViewModels
{
    public class GymClassIndexViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Starttid")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Längd")]
        public TimeSpan Duration { get; set; }

        [Display(Name = "Sluttid")]
        public DateTime EndTime { get { return (StartTime + Duration); } }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        public bool IsBooked { get; set; }

        // Navigational properties
        public ICollection<ApplicationUserGymClass> GymClassMembers { get; set; } = new List<ApplicationUserGymClass>();
    }
}
