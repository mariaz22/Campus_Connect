namespace CampusConnect.Application.DTOs.Auth;

public class AuthResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public LoginResponse? Data { get; set; }
}
