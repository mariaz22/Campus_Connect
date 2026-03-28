namespace CampusConnect.Application.DTOs.Groups;

public class SavedTaskResponse
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string? TaskDescription { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
