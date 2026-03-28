using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class GroupAnnouncement
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int AnnouncementId { get; set; }

    public int ForwardedByProfessorId { get; set; }

    public DateTime ForwardedAt { get; set; }

    public virtual Announcement Announcement { get; set; } = null!;

    public virtual User ForwardedByProfessor { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
