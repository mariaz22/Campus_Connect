namespace CampusConnect.Domain.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Ex: INFO101
    public string? Description { get; set; }
    public int Year { get; set; } = 1; // 1, 2, 3 pentru anii de studiu
    public int ProfessorId { get; set; }
    public ApplicationUser Professor { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
