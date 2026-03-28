using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Subject { get; set; } = null!;

    public int ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();

    public virtual ICollection<GroupAnnouncement> GroupAnnouncements { get; set; } = new List<GroupAnnouncement>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<GroupTask> GroupTasks { get; set; } = new List<GroupTask>();

    public virtual User Professor { get; set; } = null!;
}
