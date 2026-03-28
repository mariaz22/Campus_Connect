using CampusConnect.Domain.Entities;

public interface IActivityLoggerService
{
    Task LogActivityAsync(int userId, string activityType, string entityType, int? entityId = null, string? entityName = null, string? description = null);
    Task<List<UserActivity>> GetUserActivitiesAsync(int userId, int? limit = null);
    Task<List<UserActivity>> GetRecentActivitiesAsync(int userId, int count = 3);
}