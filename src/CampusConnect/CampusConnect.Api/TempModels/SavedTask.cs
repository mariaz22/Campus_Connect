using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class SavedTask
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TaskId { get; set; }

    public DateTime SavedAt { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual GroupTask Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
