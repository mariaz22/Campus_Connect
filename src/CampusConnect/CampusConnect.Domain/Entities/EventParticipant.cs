using System;

namespace CampusConnect.Domain.Entities
{
    public class EventParticipant
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } 
        public int EventId { get; set; }
        public Event Event { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    }
}