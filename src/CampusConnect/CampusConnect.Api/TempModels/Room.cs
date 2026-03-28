using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Capacity { get; set; }

    public string? Floor { get; set; }

    public string? Equipment { get; set; }

    public int BuildingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<RoomBookingRequest> RoomBookingRequests { get; set; } = new List<RoomBookingRequest>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
