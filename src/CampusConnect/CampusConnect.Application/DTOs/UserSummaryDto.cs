namespace CampusConnect.Application.DTOs;

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // StudentId poate fi null (de ex. pentru un Admin care nu e student)
        public string? StudentId { get; set; } 
        
        // Poate fi null dacă userul nu are poză
        public string? ProfilePictureUrl { get; set; } 

        // CRITIC: Aici vom stoca "Admin", "User" sau "Professor"
        public string Role { get; set; } = "User"; 
    }
