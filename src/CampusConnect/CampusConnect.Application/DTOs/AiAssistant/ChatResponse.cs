namespace CampusConnect.Application.DTOs.AiAssistant;

public class ChatResponse
{
    public required string Message { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<SuggestedAction>? SuggestedActions { get; set; }
}

public class SuggestedAction
{
    public required string Type { get; set; }
    public required string Label { get; set; }
    public string? Data { get; set; }
}
