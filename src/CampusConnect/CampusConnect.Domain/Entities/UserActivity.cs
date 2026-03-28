namespace CampusConnect.Domain.Entities;

public class UserActivity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string ActivityType { get; set; }
    public required string EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
