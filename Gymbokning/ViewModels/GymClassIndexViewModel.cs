using Gymbokning.Models;
using System.ComponentModel.DataAnnotations;

namespace Gymbokning.ViewModels
{
    public class GymClassIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        [Display(Name = "End Time")]
        public DateTime EndTime { get { return (StartTime + Duration); } }
        public string Description { get; set; }

        public bool IsBooked { get; set; }

        // Navigational properties
        public ICollection<ApplicationUserGymClass> GymClassMembers { get; set; } = new List<ApplicationUserGymClass>();
    }
}
