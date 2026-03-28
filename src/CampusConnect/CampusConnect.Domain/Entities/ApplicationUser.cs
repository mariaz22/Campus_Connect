using Microsoft.AspNetCore.Identity;

namespace CampusConnect.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? StudentId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public ICollection<EventParticipant> EventsJoined { get; set; } = new List<EventParticipant>();
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

}
