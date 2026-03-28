using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class SavedAnnouncement
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AnnouncementId { get; set; }

    public virtual Announcement Announcement { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
