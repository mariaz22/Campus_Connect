using CampusConnect.Application.DTOs.AiAssistant;

namespace CampusConnect.Application.Interfaces;

public interface IAiAssistantService
{
    Task<ChatResponse> ProcessMessageAsync(ChatRequest request, int? userId = null);
}
