using CampusConnect.Application.DTOs.Achievements;

namespace CampusConnect.Application.Interfaces;

public interface IAchievementService
{
    // Achievement Management (Admin only)
    Task<AchievementResponse> CreateAchievementAsync(CreateAchievementRequest request);
    Task<AchievementResponse?> UpdateAchievementAsync(int achievementId, UpdateAchievementRequest request);
    Task<bool> DeleteAchievementAsync(int achievementId);
    Task<IEnumerable<AchievementResponse>> GetAllAchievementsAsync();
    Task<AchievementResponse?> GetAchievementByIdAsync(int achievementId);

    // User Achievements
    Task<IEnumerable<UserAchievementResponse>> GetUserAchievementsAsync(int userId);
    Task<IEnumerable<UserAchievementResponse>> GetMyAchievementsAsync();
    Task<bool> GrantAchievementAsync(int userId, int achievementId);

    // Auto-grant achievements
    Task CheckAndGrantTaskAchievementsAsync(int userId);
    Task CheckAndGrantGroupAchievementAsync(int userId);
    Task CheckAndGrantEventAchievementAsync(int userId);
}
