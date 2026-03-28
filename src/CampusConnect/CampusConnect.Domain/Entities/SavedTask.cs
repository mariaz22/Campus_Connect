namespace CampusConnect.Domain.Entities;

public class SavedTask
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public int TaskId { get; set; }
    public GroupTask Task { get; set; } = null!;
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
}
