namespace CampusConnect.Application.DTOs.Subjects;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int GradesCount { get; set; }
}
