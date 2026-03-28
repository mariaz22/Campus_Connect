using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Application.DTOs.AiAssistant;

public class ChatRequest
{
    [Required(ErrorMessage = "Message is required")]
    [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public required string Message { get; set; }

    public List<ChatMessage>? ConversationHistory { get; set; }
}

public class ChatMessage
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}
