using CampusConnect.Application.DTOs.Achievements;
using CampusConnect.Application.Interfaces;
using CampusConnect.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusConnect.Api.Controllers.Social;

[ApiController]
[Route("api/[controller]")]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService _achievementService;
    private readonly IActivityLoggerService _activityLogger;

    public AchievementController(IAchievementService achievementService, IActivityLoggerService activityLogger)
    {
        _achievementService = achievementService;
        _activityLogger=activityLogger;
    }
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst("id")?.Value;
        }

        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        }

        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        return null;
    }
    // GET: api/achievement
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AchievementResponse>>> GetAllAchievements()
    {
        var achievements = await _achievementService.GetAllAchievementsAsync();
        return Ok(achievements);
    }

    // GET: api/achievement/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AchievementResponse>> GetAchievementById(int id)
    {
        var achievement = await _achievementService.GetAchievementByIdAsync(id);
        if (achievement == null)
            return NotFound();

        return Ok(achievement);
    }

    // GET: api/achievement/user/{userId}
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<UserAchievementResponse>>> GetUserAchievements(int userId)
    {
        var achievements = await _achievementService.GetUserAchievementsAsync(userId);
        return Ok(achievements);
    }

    // GET: api/achievement/my
    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<UserAchievementResponse>>> GetMyAchievements()
    {
        try
        {
            var achievements = await _achievementService.GetMyAchievementsAsync();
            return Ok(achievements);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // POST: api/achievement
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AchievementResponse>> CreateAchievement([FromBody] CreateAchievementRequest request)
    {
        var userId = GetCurrentUserId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var achievement = await _achievementService.CreateAchievementAsync(request);
        await _activityLogger.LogActivityAsync(userId.Value, "Create", "Achievement", achievement.Id, achievement.Title, "Created a new achievement");
        return CreatedAtAction(nameof(GetAchievementById), new { id = achievement.Id }, achievement);
    }

    // PUT: api/achievement/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<AchievementResponse>> UpdateAchievement(int id, [FromBody] UpdateAchievementRequest request)
    {
        var userId = GetCurrentUserId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var achievement = await _achievementService.UpdateAchievementAsync(id, request);
        if (achievement == null)
            return NotFound();
        await _activityLogger.LogActivityAsync(userId.Value, "Update", "Achievement", achievement.Id, achievement.Title, "Updated an achievement");
        return Ok(achievement);
    }

    // DELETE: api/achievement/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAchievement(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _achievementService.DeleteAchievementAsync(id);

        if (!result)
            return NotFound();
        await _activityLogger.LogActivityAsync(userId.Value, "Delete", "Achievement", id, null, "Deleted an achievement");
        return NoContent();
    }
}
