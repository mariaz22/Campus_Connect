using Microsoft.AspNetCore.Http;

namespace CampusConnect.Application.DTOs.Groups;

public class CreateCourseMaterialRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required IFormFile File { get; set; }
    public int GroupId { get; set; }
}
