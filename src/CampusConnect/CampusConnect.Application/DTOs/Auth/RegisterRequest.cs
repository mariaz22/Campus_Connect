using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email este obligatoriu")]
    [EmailAddress(ErrorMessage = "Format email invalid")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Parola este obligatorie")]
    [MinLength(8, ErrorMessage = "Parola trebuie să aibă minim 8 caractere")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirmarea parolei este obligatorie")]
    [Compare("Password", ErrorMessage = "Parolele nu se potrivesc")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Prenumele este obligatoriu")]
    [StringLength(100, ErrorMessage = "Prenumele nu poate depăși 100 de caractere")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Numele este obligatoriu")]
    [StringLength(100, ErrorMessage = "Numele nu poate depăși 100 de caractere")]
    public required string LastName { get; set; }

    [StringLength(50)]
    public string? StudentId { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
