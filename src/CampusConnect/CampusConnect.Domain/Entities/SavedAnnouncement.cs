using System;

namespace CampusConnect.Domain.Entities
{
    public class SavedAnnouncement
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int AnnouncementId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Announcement Announcement { get; set; } = null!;
    }
}
