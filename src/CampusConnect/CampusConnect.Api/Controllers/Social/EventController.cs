using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CampusConnect.Api.Controllers.Social
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAchievementService _achievementService;
        private readonly IActivityLoggerService _activityLogger;

        public EventController(ApplicationDbContext context, IAchievementService achievementService, IActivityLoggerService activityLogger)
        {
            _context = context;
            _achievementService = achievementService;
            _activityLogger = activityLogger;
        }
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<Event>>> GetUpcomingEvents([FromQuery] string? search = null)
        {
            var query = _context.Events
                .Where(e => e.Date > DateTime.UtcNow)
                .Include(e => e.Participants)
                .AsQueryable(); 
                
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(search) 
                                    || e.Description.ToLower().Contains(search));
            }

            var events = await query
                .OrderBy(e => e.Date)
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Participants) 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();
            return Ok(eventItem);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> Create(Event eventItem)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            var isProfessor = User.IsInRole("Professor");

            if (!isAdmin && !isProfessor)
            {
                return Unauthorized(); 
            }

            eventItem.OrganizerId = userId.Value;
            eventItem.DateCreated = DateTime.UtcNow;

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            await _activityLogger.LogActivityAsync(userId.Value, "Create", "Event", eventItem.Id, eventItem.Title, "Created a new event");
            return CreatedAtAction(nameof(GetById), new { id = eventItem.Id }, eventItem);
        }

       [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, Event updatedEvent)
        {
            var userId = GetCurrentUserId();
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound();
            var isAdmin = User.IsInRole("Admin");

            if (userId != existingEvent.OrganizerId && !isAdmin) 
            {
                return Unauthorized(); 
            }

            existingEvent.Title = updatedEvent.Title;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.Date = updatedEvent.Date;
            existingEvent.Category = updatedEvent.Category;
            
            await _context.SaveChangesAsync();
            await _activityLogger.LogActivityAsync(userId.Value, "Update", "Event", existingEvent.Id, existingEvent.Title, "Updated an event");
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null) return NotFound();
            var isAdmin = User.IsInRole("Admin");

            if (userId != eventItem.OrganizerId && !isAdmin)
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            await _activityLogger.LogActivityAsync(userId.Value, "Delete", "Event", eventItem.Id, eventItem.Title, "Deleted an event");
            return NoContent();
        }

        [HttpPost("{id}/join")]
        [Authorize]
        public async Task<IActionResult> Participate(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var eventItem = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();
            await _activityLogger.LogActivityAsync(userId.Value, "Participate", "Event", eventItem.Id, eventItem.Title, "Participated in an event");
            if (eventItem.Participants.Any(p => p.UserId == userId.Value))
            {
                return BadRequest("You are already participating in this event");
            }

            var participation = new EventParticipant
            {
                EventId = id,
                UserId = userId.Value,
                JoinedAt = DateTime.UtcNow
            };

            _context.EventParticipants.Add(participation);
            await _context.SaveChangesAsync();

            await _achievementService.CheckAndGrantEventAchievementAsync(userId.Value);

            return Ok();
        }

        [HttpDelete("{id}/leave")]
        [Authorize]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var eventItem = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();

            var participation = eventItem.Participants.FirstOrDefault(p => p.UserId == userId.Value);
            if (participation == null)
            {
                return BadRequest("You are not participating in this event.");
            }

            _context.EventParticipants.Remove(participation);
            await _context.SaveChangesAsync();
            await _activityLogger.LogActivityAsync(userId.Value, "Withdraw", "Event", eventItem.Id, eventItem.Title, "Withdrew from an event");
            return Ok();
        }

        [HttpPost("{id}/save")]
        [Authorize]
        public async Task<IActionResult> SaveEvent(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();
            var alreadySaved = await _context.SavedEvents
                .AnyAsync(se => se.UserId == userId.Value && se.EventId == id);

            if (alreadySaved)
            {
                return BadRequest("The event is already saved.");
            }

            var savedEvent = new SavedEvent
            {
                UserId = userId.Value,
                EventId = id
            };

            _context.SavedEvents.Add(savedEvent);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}/unsave")]
        [Authorize]
        public async Task<IActionResult> UnsaveEvent(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var savedEvent = await _context.SavedEvents
                .FirstOrDefaultAsync(se => se.UserId == userId.Value && se.EventId == id);

            if (savedEvent == null)
            {
                return BadRequest("The event is not saved.");
            }

            _context.SavedEvents.Remove(savedEvent);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("saved")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Event>>> GetSavedEvents()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var savedEvents = await _context.SavedEvents
                .Where(se => se.UserId == userId.Value)
                .Include(se => se.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(se => se.Event)
                    .ThenInclude(e => e.Participants)
                .Select(se => se.Event)
                .ToListAsync();

            return Ok(savedEvents);
        }

        [HttpGet("my-events")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Event>>> GetMyParticipatingEvents()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var participatingEvents = await _context.EventParticipants
                .Where(ep => ep.UserId == userId.Value)
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.Participants)
                .Select(ep => ep.Event)
                .ToListAsync();

            return Ok(participatingEvents);
        }

    }
}