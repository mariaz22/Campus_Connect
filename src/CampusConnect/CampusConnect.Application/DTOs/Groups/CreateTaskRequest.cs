namespace CampusConnect.Application.DTOs.Groups;

public class CreateTaskRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}
