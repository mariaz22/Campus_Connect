namespace CampusConnect.Application.DTOs.Assistant;

public class ChatResponse
{
    public required string Message { get; set; }
    public required string SessionId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public List<SuggestedAction>? SuggestedActions { get; set; }
}

public class SuggestedAction
{
    public required string Label { get; set; }
    public required string ActionType { get; set; } // "navigate", "link", "query"
    public string? Payload { get; set; }
}
