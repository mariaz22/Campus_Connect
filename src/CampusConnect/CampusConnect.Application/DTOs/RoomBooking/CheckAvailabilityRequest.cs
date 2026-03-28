namespace CampusConnect.Application.DTOs.RoomBooking;

public class CheckAvailabilityRequest
{
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class AvailabilityCheckResult
{
    public bool IsAvailable { get; set; }
    public bool IsTimeValid { get; set; }
    public string? TimeError { get; set; }
    public string? ConflictError { get; set; }
    public List<ConflictInfo> Conflicts { get; set; } = new();
}

public class ConflictInfo
{
    public string Title { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Type { get; set; } = ""; // "Schedule" or "PendingRequest"
}
