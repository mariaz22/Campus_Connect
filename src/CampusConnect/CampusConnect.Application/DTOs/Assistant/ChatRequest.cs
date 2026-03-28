using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Application.DTOs.Assistant;

public class ChatRequest
{
    [Required(ErrorMessage = "Message is required")]
    public required string Message { get; set; }

    public string? SessionId { get; set; }
}
