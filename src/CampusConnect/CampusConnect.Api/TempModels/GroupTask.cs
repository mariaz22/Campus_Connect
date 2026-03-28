using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class GroupTask
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int GroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public int CreatedByProfessorId { get; set; }

    public virtual User CreatedByProfessor { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<SavedTask> SavedTasks { get; set; } = new List<SavedTask>();
}
