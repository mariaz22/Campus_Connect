namespace CampusConnect.Application.DTOs.Groups;

public class GroupTaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string CreatedByProfessorName { get; set; } = string.Empty;
    public bool IsSavedByUser { get; set; }
    public bool IsCompleted { get; set; }
}
