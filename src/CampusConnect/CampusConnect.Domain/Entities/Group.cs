namespace CampusConnect.Domain.Entities;

public class Group
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Subject { get; set; } // Materia
    public int ProfessorId { get; set; }
    public ApplicationUser Professor { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Relații
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<GroupTask> Tasks { get; set; } = new List<GroupTask>();
    public ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();
}
