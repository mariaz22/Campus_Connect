namespace CampusConnect.Domain.Entities;

public class Achievement
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
