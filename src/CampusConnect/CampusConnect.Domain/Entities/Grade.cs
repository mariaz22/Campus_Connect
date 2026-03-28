namespace CampusConnect.Domain.Entities;

public class Grade
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public int StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
    public decimal Value { get; set; } // Nota (ex: 10, 9.5, etc.)
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int CreatedByProfessorId { get; set; }
    public ApplicationUser CreatedByProfessor { get; set; } = null!;
}
