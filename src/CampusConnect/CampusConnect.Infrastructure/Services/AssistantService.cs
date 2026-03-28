using CampusConnect.Application.DTOs.Assistant;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Domain.Services;
using CampusConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class AssistantService : IAssistantService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOpenAiService _openAiService;

    public AssistantService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager,
        IOpenAiService openAiService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _openAiService = openAiService;
    }

    public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request)
    {
        var userContext = await GetUserContextAsync();
        return await _openAiService.GenerateResponseAsync(request.Message, userContext, request.SessionId);
    }

    public async Task<UserContextDto> GetUserContextAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        // Get user's groups
        var groupNames = await _context.GroupMembers
            .Where(gm => gm.UserId == userId.Value)
            .Include(gm => gm.Group)
            .Where(gm => gm.Group.IsActive)
            .Select(gm => gm.Group.Name)
            .ToListAsync();

        // Get user's subjects (from grades)
        var subjectNames = await _context.Grades
            .Where(g => g.StudentId == userId.Value)
            .Include(g => g.Subject)
            .Select(g => g.Subject.Name)
            .Distinct()
            .ToListAsync();

        // Get campus stats
        var buildingCount = await _context.Buildings.CountAsync(b => b.IsActive);
        var roomCount = await _context.Rooms.CountAsync(r => r.IsActive);

        return new UserContextDto
        {
            UserId = userId.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Role = role,
            GroupNames = groupNames,
            SubjectNames = subjectNames,
            TotalBuildings = buildingCount,
            TotalRooms = roomCount
        };
    }

    public async Task<IEnumerable<string>> GetSuggestedQuestionsAsync()
    {
        var userContext = await GetUserContextAsync();

        var suggestions = new List<string>
        {
            "How do I join a study group?",
            "Where can I find campus buildings?",
            "How do I check my grades?",
            "What events are coming up?",
            "How do I view my tasks?"
        };

        // Personalize suggestions based on user context
        if (userContext.GroupNames.Count == 0)
        {
            suggestions.Insert(0, "How do I join my first group?");
        }

        if (userContext.Role == "Professor" || userContext.Role == "Admin")
        {
            suggestions.Add("How do I create a new group?");
            suggestions.Add("How do I post an announcement?");
        }

        return suggestions.Take(5);
    }
}
