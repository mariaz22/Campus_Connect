using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class EventParticipant
{
    public int UserId { get; set; }

    public int EventId { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
