using CampusConnect.Application.DTOs.Assistant;

namespace CampusConnect.Application.Interfaces;

public interface IOpenAiService
{
    Task<ChatResponse> GenerateResponseAsync(string userMessage, UserContextDto userContext, string? sessionId = null);
}
