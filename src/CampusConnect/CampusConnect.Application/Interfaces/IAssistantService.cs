using CampusConnect.Application.DTOs.Assistant;

namespace CampusConnect.Application.Interfaces;

public interface IAssistantService
{
    Task<ChatResponse> ProcessMessageAsync(ChatRequest request);
    Task<UserContextDto> GetUserContextAsync();
    Task<IEnumerable<string>> GetSuggestedQuestionsAsync();
}
