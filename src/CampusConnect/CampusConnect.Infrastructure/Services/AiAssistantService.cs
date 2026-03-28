using CampusConnect.Application.DTOs.AiAssistant;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class AiAssistantService : IAiAssistantService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOpenAiService _openAiService;

    public AiAssistantService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IOpenAiService openAiService)
    {
        _context = context;
        _userManager = userManager;
        _openAiService = openAiService;
    }

    public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request, int? userId = null)
    {
        try
        {
            var userContext = await GetUserContextAsync(userId);
            var response = await _openAiService.GenerateResponseAsync(request.Message, userContext, null);

            return new ChatResponse
            {
                Message = response.Message,
                Success = true,
                SuggestedActions = response.SuggestedActions?.Select(a => new SuggestedAction
                {
                    Type = a.ActionType,
                    Label = a.Label,
                    Data = a.Payload
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            return new ChatResponse
            {
                Message = "I'm having trouble processing your request. Please try again later.",
                Success = false,
                Error = ex.Message
            };
        }
    }

    private async Task<Application.DTOs.Assistant.UserContextDto> GetUserContextAsync(int? userId)
    {
        if (userId == null)
        {
            return new Application.DTOs.Assistant.UserContextDto
            {
                UserId = 0,
                FirstName = "Guest",
                LastName = "",
                Email = "",
                Role = "Guest",
                GroupNames = new List<string>(),
                SubjectNames = new List<string>(),
                TotalBuildings = 0,
                TotalRooms = 0
            };
        }

        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        if (user == null)
        {
            return new Application.DTOs.Assistant.UserContextDto
            {
                UserId = 0,
                FirstName = "Unknown",
                LastName = "",
                Email = "",
                Role = "User",
                GroupNames = new List<string>(),
                SubjectNames = new List<string>(),
                TotalBuildings = 0,
                TotalRooms = 0
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        var groupNames = await _context.GroupMembers
            .Where(gm => gm.UserId == userId.Value)
            .Include(gm => gm.Group)
            .Where(gm => gm.Group.IsActive)
            .Select(gm => gm.Group.Name)
            .ToListAsync();

        var subjectNames = await _context.Grades
            .Where(g => g.StudentId == userId.Value)
            .Include(g => g.Subject)
            .Select(g => g.Subject.Name)
            .Distinct()
            .ToListAsync();

        var buildingCount = await _context.Buildings.CountAsync(b => b.IsActive);
        var roomCount = await _context.Rooms.CountAsync(r => r.IsActive);

        return new Application.DTOs.Assistant.UserContextDto
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
}
