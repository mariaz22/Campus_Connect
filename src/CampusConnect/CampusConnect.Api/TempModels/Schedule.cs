using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Schedule
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? RecurrencePattern { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    public int CreatedByProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual User CreatedByProfessor { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
