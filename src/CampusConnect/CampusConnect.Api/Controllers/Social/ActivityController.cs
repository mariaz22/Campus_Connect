using CampusConnect.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusConnect.Api.Controllers.Social;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IActivityLoggerService _activityLogger;

    public ActivityController(IActivityLoggerService activityLogger)
    {
        _activityLogger = activityLogger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentActivities()
    {
        var userId = GetCurrentUserId();
        var activities = await _activityLogger.GetRecentActivitiesAsync(userId, 3);
        return Ok(activities);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllActivities()
    {
        var userId = GetCurrentUserId();
        var activities = await _activityLogger.GetUserActivitiesAsync(userId);
        return Ok(activities);
    }

    [HttpGet]
    public async Task<IActionResult> GetActivities([FromQuery] int? limit = null)
    {
        var userId = GetCurrentUserId();
        var activities = await _activityLogger.GetUserActivitiesAsync(userId, limit);
        return Ok(activities);
    }
}
