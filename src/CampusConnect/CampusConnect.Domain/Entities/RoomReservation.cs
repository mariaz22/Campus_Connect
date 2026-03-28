namespace CampusConnect.Domain.Entities;

public enum ReservationStatus
{
    Pending,
    Approved,
    Rejected
}

public class RoomReservation
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RequestedByUserId { get; set; }
    public ApplicationUser RequestedByUser { get; set; } = null!;
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public int? ProcessedByAdminId { get; set; }
    public ApplicationUser? ProcessedByAdmin { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
