using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class UserActivity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public int? EntityId { get; set; }

    public string? EntityName { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
