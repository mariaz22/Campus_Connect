namespace CampusConnect.Domain.Entities;

public class CourseMaterial
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string FileName { get; set; }
    public required string FileUrl { get; set; } // URL sau path către fișier
    public required string FileType { get; set; } // pdf, doc, ppt, etc.
    public long FileSize { get; set; } // în bytes
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public int UploadedByProfessorId { get; set; }
    public ApplicationUser UploadedByProfessor { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
