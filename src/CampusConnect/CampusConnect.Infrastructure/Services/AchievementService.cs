using CampusConnect.Application.DTOs.Achievements;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Domain.Services;
using CampusConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class AchievementService : IAchievementService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AchievementService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AchievementResponse> CreateAchievementAsync(CreateAchievementRequest request)
    {
        var achievement = new Achievement
        {
            Title = request.Title,
            Description = request.Description,
            Icon = request.Icon,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();

        return MapToAchievementResponse(achievement);
    }

    public async Task<AchievementResponse?> UpdateAchievementAsync(int achievementId, UpdateAchievementRequest request)
    {
        var achievement = await _context.Achievements.FindAsync(achievementId);
        if (achievement == null)
            return null;

        achievement.Title = request.Title;
        achievement.Description = request.Description;
        achievement.Icon = request.Icon;
        achievement.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return MapToAchievementResponse(achievement);
    }

    public async Task<bool> DeleteAchievementAsync(int achievementId)
    {
        var achievement = await _context.Achievements.FindAsync(achievementId);
        if (achievement == null)
            return false;

        _context.Achievements.Remove(achievement);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<AchievementResponse>> GetAllAchievementsAsync()
    {
        var achievements = await _context.Achievements
            .Where(a => a.IsActive)
            .OrderBy(a => a.Id)
            .ToListAsync();

        return achievements.Select(MapToAchievementResponse);
    }

    public async Task<AchievementResponse?> GetAchievementByIdAsync(int achievementId)
    {
        var achievement = await _context.Achievements.FindAsync(achievementId);
        if (achievement == null)
            return null;

        return MapToAchievementResponse(achievement);
    }

    public async Task<IEnumerable<UserAchievementResponse>> GetUserAchievementsAsync(int userId)
    {
        var userAchievements = await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId && ua.Achievement.IsActive)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();

        return userAchievements.Select(MapToUserAchievementResponse);
    }

    public async Task<IEnumerable<UserAchievementResponse>> GetMyAchievementsAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        return await GetUserAchievementsAsync(userId.Value);
    }

    public async Task<bool> GrantAchievementAsync(int userId, int achievementId)
    {
        // Check if user already has this achievement
        var existingAchievement = await _context.UserAchievements
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

        if (existingAchievement != null)
            return false; // Already has this achievement

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task CheckAndGrantTaskAchievementsAsync(int userId)
    {
        // Count completed tasks
        var completedTasksCount = await _context.SavedTasks
            .Where(st => st.UserId == userId && st.IsCompleted)
            .CountAsync();

        // Achievement ID 1: First Steps (Complete your first task)
        if (completedTasksCount >= 1)
        {
            await GrantAchievementAsync(userId, 1);
        }

        // Achievement ID 2: Task Master (Complete 5 tasks)
        if (completedTasksCount >= 5)
        {
            await GrantAchievementAsync(userId, 2);
        }

        // Achievement ID 3: Task Legend (Complete 10 tasks)
        if (completedTasksCount >= 10)
        {
            await GrantAchievementAsync(userId, 3);
        }
    }

    public async Task CheckAndGrantGroupAchievementAsync(int userId)
    {
        // Check if user has joined at least one group
        var hasJoinedGroup = await _context.GroupMembers
            .AnyAsync(gm => gm.UserId == userId);

        // Achievement ID 4: Team Player (Join your first group)
        if (hasJoinedGroup)
        {
            await GrantAchievementAsync(userId, 4);
        }
    }

    public async Task CheckAndGrantEventAchievementAsync(int userId)
    {
        // Check if user has attended at least one event
        var hasAttendedEvent = await _context.EventParticipants
            .AnyAsync(ep => ep.UserId == userId);

        // Achievement ID 5: Social Butterfly (Attend your first event)
        if (hasAttendedEvent)
        {
            await GrantAchievementAsync(userId, 5);
        }
    }

    private AchievementResponse MapToAchievementResponse(Achievement achievement)
    {
        return new AchievementResponse
        {
            Id = achievement.Id,
            Title = achievement.Title,
            Description = achievement.Description,
            Icon = achievement.Icon,
            CreatedAt = achievement.CreatedAt,
            IsActive = achievement.IsActive
        };
    }

    private UserAchievementResponse MapToUserAchievementResponse(UserAchievement userAchievement)
    {
        return new UserAchievementResponse
        {
            Id = userAchievement.Id,
            AchievementId = userAchievement.AchievementId,
            Title = userAchievement.Achievement.Title,
            Description = userAchievement.Achievement.Description,
            Icon = userAchievement.Achievement.Icon,
            UnlockedAt = userAchievement.UnlockedAt
        };
    }
}
