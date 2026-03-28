namespace CampusConnect.Application.DTOs.Assistant;

public class UserContextDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> GroupNames { get; set; } = new();
    public List<string> SubjectNames { get; set; } = new();
    public int TotalBuildings { get; set; }
    public int TotalRooms { get; set; }
}
