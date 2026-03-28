using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Application.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email este obligatoriu")]
    [EmailAddress(ErrorMessage = "Format email invalid")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Parola este obligatorie")]
    public required string Password { get; set; }
}
