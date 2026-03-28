namespace CampusConnect.Application.DTOs.Subjects;

public class UpdateSubjectRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
