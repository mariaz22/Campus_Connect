namespace CampusConnect.Application.DTOs.CampusMap;

public class RoomReservationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RequestedByUserId { get; set; }
    public string RequestedByUserName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? ProcessedByAdminId { get; set; }
    public string? ProcessedByAdminName { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class CreateReservationRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required int RoomId { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}

public class ProcessReservationRequest
{
    public required bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}
