namespace CampusConnect.Domain.Entities;

public class RoomBookingRequest
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? RecurrencePattern { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }

    public int RequestedByUserId { get; set; }
    public ApplicationUser RequestedByUser { get; set; } = null!;

    public BookingRequestStatus Status { get; set; } = BookingRequestStatus.Pending;

    public int? ReviewedByAdminId { get; set; }
    public ApplicationUser? ReviewedByAdmin { get; set; }

    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public enum BookingRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
