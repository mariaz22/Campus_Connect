using System;

namespace CampusConnect.Domain.Entities
{
    public class SavedEvent
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int EventId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}
