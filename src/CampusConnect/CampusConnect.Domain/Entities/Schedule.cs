namespace CampusConnect.Domain.Entities;

public class Schedule
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
    public int CreatedByProfessorId { get; set; }
    public ApplicationUser CreatedByProfessor { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
