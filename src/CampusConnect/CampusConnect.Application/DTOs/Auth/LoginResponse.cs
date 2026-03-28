namespace CampusConnect.Application.DTOs.Auth;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required string Role { get; set; }
    public string? StudentId { get; set; }
}
