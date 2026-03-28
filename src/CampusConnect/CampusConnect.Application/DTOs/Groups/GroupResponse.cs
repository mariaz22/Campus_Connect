namespace CampusConnect.Application.DTOs.Groups;

public class GroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int MembersCount { get; set; }
    public int TasksCount { get; set; }
    public bool IsUserMember { get; set; }
}
