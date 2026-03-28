namespace CampusConnect.Application.DTOs.Achievements;

public class UserAchievementResponse
{
    public int Id { get; set; }
    public int AchievementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }
}
