using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class RoomBookingRequest
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? RecurrencePattern { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    public int RequestedByUserId { get; set; }

    public int Status { get; set; }

    public int? ReviewedByAdminId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual User RequestedByUser { get; set; } = null!;

    public virtual User? ReviewedByAdmin { get; set; }

    public virtual Room Room { get; set; } = null!;
}
