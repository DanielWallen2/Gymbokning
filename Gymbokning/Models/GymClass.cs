﻿namespace Gymbokning.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime { get { return (StartTime + Duration); } }
        public string Description { get; set; }

        // Navigational properties
        public ICollection<ApplicationUserGymClass> GymClassMembers { get; set; }

    }
}
