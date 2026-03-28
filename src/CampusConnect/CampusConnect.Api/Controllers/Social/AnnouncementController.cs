using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CampusConnect.Api.Controllers.Social;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLoggerService _activityLogger;

    public AnnouncementsController(ApplicationDbContext context, IActivityLoggerService activityLogger)
    {
        _context = context;
        _activityLogger = activityLogger;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Announcement>>> GetAll([FromQuery] string? category, [FromQuery] string? search)
    {
        var query = _context.Announcements.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(search) 
                                || a.Content.ToLower().Contains(search));
        }

        var list = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(list);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();
        return Ok(announcement);
    }

[Authorize] 
[HttpPost]
public async Task<IActionResult> Create(Announcement announcement)
{
    var userId = GetCurrentUserId();
    if (userId == null) return Unauthorized();

    announcement.CreatedByUserId = userId.Value; 
    if (announcement.CreatedAt == default) announcement.CreatedAt = DateTime.UtcNow;

    _context.Announcements.Add(announcement);
    await _context.SaveChangesAsync();
    var subscribers = await _context.CategorySubscriptions
            .Where(s => s.Category == announcement.Category && s.UserId != userId.Value)
            .Select(s => s.UserId)
            .Distinct()
            .ToListAsync();

        if (subscribers.Any())
        {
            var notifications = subscribers.Select(subUserId => new Notification
            {
                UserId = subUserId,
                Message = $"Anunț nou în categoria {announcement.Category}: {announcement.Title}",
                RelatedEntityType = "Announcement",
                RelatedEntityId = announcement.Id,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            _context.Notifications.AddRange(notifications);

            await _context.SaveChangesAsync();
        }
        await _activityLogger.LogActivityAsync(userId.Value, "Create", "Announcement", announcement.Id, announcement.Title, "Created a new announcement");
    return CreatedAtAction(nameof(GetById), new { id = announcement.Id }, announcement);
}

    [Authorize]
    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeToCategory([FromBody] string category)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(category)) return BadRequest("Invalid category");

        var exists = await _context.CategorySubscriptions
            .AnyAsync(s => s.UserId == userId.Value && s.Category == category);

        if (exists) return BadRequest("Already subscribed.");

        _context.CategorySubscriptions.Add(new CategorySubscription 
        { 
            UserId = userId.Value, 
            Category = category 
        });
        
        await _context.SaveChangesAsync();
        await _activityLogger.LogActivityAsync(userId.Value, "Subscribe", "CategorySubscription", null, category, "Subscribed to a category");
        return Ok(new { message = $"Subscribed to {category}" });
    }

    // 3. ENDPOINT PENTRU DEZABONARE
    [Authorize]
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> UnsubscribeFromCategory([FromBody] string category)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var sub = await _context.CategorySubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId.Value && s.Category == category);

        if (sub == null) return NotFound("Subscription not found.");

        _context.CategorySubscriptions.Remove(sub);
        await _context.SaveChangesAsync();
        await _activityLogger.LogActivityAsync(userId.Value, "Unsubscribe", "CategorySubscription", null, category, "Unsubscribed from a category");
        return Ok(new { message = $"Unsubscribed from {category}" });
    }

    // 4. LISTA CATEGORIILOR ABONATE (pentru UI)
    [Authorize]
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetMySubscriptions()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var categories = await _context.CategorySubscriptions
            .Where(s => s.UserId == userId.Value)
            .Select(s => s.Category)
            .ToListAsync();

        return Ok(categories);
    }
[Authorize]
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, Announcement updated)
{
    var announcement = await _context.Announcements.FindAsync(id);
    if (announcement == null) return NotFound();
    var userId = GetCurrentUserId();
    var isAdmin = User.IsInRole("Admin"); 
    
    if (announcement.CreatedByUserId != userId && !isAdmin)
    {
        return Forbid(); 
    }

    announcement.Title = updated.Title;
    announcement.Content = updated.Content;
    await _context.SaveChangesAsync();
    await _activityLogger.LogActivityAsync(userId.Value, "Update", "Announcement", announcement.Id, announcement.Title, "Updated an announcement");
    return NoContent();
}

[Authorize]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var announcement = await _context.Announcements.FindAsync(id);
    if (announcement == null) return NotFound();

    var userId = GetCurrentUserId();
    var isAdmin = User.IsInRole("Admin"); 

    if (announcement.CreatedByUserId != userId && !isAdmin)
    {
        return Forbid();
    }

    _context.Announcements.Remove(announcement);
    await _context.SaveChangesAsync();
    await _activityLogger.LogActivityAsync(userId.Value, "Delete", "Announcement", announcement.Id, announcement.Title, "Deleted an announcement");
    return NoContent();
}


    [Authorize]
    [HttpPost("{id}/bookmark")]
    public async Task<IActionResult> Bookmark(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var exists = await _context.SavedAnnouncements
            .AnyAsync(sa => sa.UserId == userId.Value && sa.AnnouncementId == id);

        if (exists)
        {
            return BadRequest(new { message = "Announcement already bookmarked." });
        }

        var saved = new SavedAnnouncement
        {
            UserId = userId.Value,
            AnnouncementId = id
        };

        _context.SavedAnnouncements.Add(saved);
        await _context.SaveChangesAsync();
        await _activityLogger.LogActivityAsync(userId.Value, "Bookmark", "Announcement", id, null, "Bookmarked an announcement");
        return Ok();
    }

    
    [Authorize]
    [HttpDelete("{id}/bookmark")]
    public async Task<IActionResult> Unbookmark(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var saved = await _context.SavedAnnouncements
            .FirstOrDefaultAsync(sa => sa.UserId == userId.Value && sa.AnnouncementId == id);

        if (saved == null) return NotFound();

        _context.SavedAnnouncements.Remove(saved);
        await _context.SaveChangesAsync();
        await _activityLogger.LogActivityAsync(userId.Value, "Unbookmark", "Announcement", id, null, "Unbookmarked an announcement");
        return NoContent();
    }

    
    [Authorize]
    [HttpGet("saved")]
    public async Task<ActionResult<IEnumerable<Announcement>>> GetSaved()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var announcements = await _context.SavedAnnouncements
            .Where(sa => sa.UserId == userId.Value)
            .Select(sa => sa.Announcement)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        
        await _activityLogger.LogActivityAsync(userId.Value, "GetSaved", "Announcement", null, null, "Retrieved saved announcements");
        return Ok(announcements);
    }

}
