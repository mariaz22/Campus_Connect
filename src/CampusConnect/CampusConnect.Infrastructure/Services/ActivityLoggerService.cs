using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class ActivityLoggerService : IActivityLoggerService
{
    private readonly ApplicationDbContext _context;

    public ActivityLoggerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActivityAsync(int userId, string activityType, string entityType, int? entityId = null, string? entityName = null, string? description = null)
    {
        var activity = new UserActivity
        {
            UserId = userId,
            ActivityType = activityType,
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserActivities.Add(activity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserActivity>> GetUserActivitiesAsync(int userId, int? limit = null)
    {
        var query = _context.UserActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt);

        if (limit.HasValue)
        {
            return await query.Take(limit.Value).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<List<UserActivity>> GetRecentActivitiesAsync(int userId, int count = 3)
    {
        return await GetUserActivitiesAsync(userId, count);
    }
}
