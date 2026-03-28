using CampusConnect.Application.DTOs.Auth;

namespace CampusConnect.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> ConfirmEmailAsync(int userId, string token);
    Task<AuthResult> ResendEmailConfirmationAsync(string email);
}
