namespace CampusConnect.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? Capacity { get; set; }
    public string? Floor { get; set; }
    public string? Equipment { get; set; }
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
