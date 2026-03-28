using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Application.DTOs; 

public class UpdateUserProfileRequest
{

    [Required(ErrorMessage = "Prenumele este obligatoriu.")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Numele de familie este obligatoriu.")]
    public required string LastName { get; set; }
    public string? ProfilePictureUrl { get; set; } 
    public string? DateOfBirth { get; set; }
    public string? StudentId { get; set; }
}