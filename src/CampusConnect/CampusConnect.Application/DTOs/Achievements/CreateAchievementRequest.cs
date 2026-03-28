using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Application.DTOs.Achievements;

public class CreateAchievementRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Icon { get; set; } = string.Empty;
}
