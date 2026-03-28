namespace CampusConnect.Application.DTOs.Groups;

public class CreateGroupRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Subject { get; set; }
}
