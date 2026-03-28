namespace CampusConnect.Application.DTOs.Groups;

public class CourseMaterialDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string FileName { get; set; }
    public required string FileUrl { get; set; }
    public required string FileType { get; set; }
    public long FileSize { get; set; }
    public int GroupId { get; set; }
    public int UploadedByProfessorId { get; set; }
    public string? UploadedByProfessorName { get; set; }
    public DateTime UploadedAt { get; set; }
}
