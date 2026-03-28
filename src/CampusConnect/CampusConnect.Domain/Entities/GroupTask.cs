namespace CampusConnect.Domain.Entities;

public class GroupTask
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public int CreatedByProfessorId { get; set; }
    public ApplicationUser CreatedByProfessor { get; set; } = null!;
    
    // Relații
    public ICollection<SavedTask> SavedByUsers { get; set; } = new List<SavedTask>();
}
