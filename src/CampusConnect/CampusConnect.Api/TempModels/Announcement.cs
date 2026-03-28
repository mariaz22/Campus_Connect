using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Announcement
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<GroupAnnouncement> GroupAnnouncements { get; set; } = new List<GroupAnnouncement>();

    public virtual ICollection<SavedAnnouncement> SavedAnnouncements { get; set; } = new List<SavedAnnouncement>();
}
