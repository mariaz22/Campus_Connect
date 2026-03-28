namespace CampusConnect.Application.DTOs.CampusMap;

public enum RoomStatus
{
    Free,
    Occupied,
    OccupiedSoon,
    Unknown
}

public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public string? Floor { get; set; }
    public string? Equipment { get; set; }
    public int BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public RoomStatus CurrentStatus { get; set; }
    public DateTime? OccupiedUntil { get; set; }
    public DateTime? NextOccupiedAt { get; set; }
}

public class RoomDetailsDto : RoomDto
{
    public List<ScheduleDto> TodaySchedules { get; set; } = new();
}

public class CreateRoomRequest
{
    public required string Name { get; set; }
    public int? Capacity { get; set; }
    public string? Floor { get; set; }
    public string? Equipment { get; set; }
    public required int BuildingId { get; set; }
}

public class UpdateRoomRequest
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? Floor { get; set; }
    public string? Equipment { get; set; }
}
