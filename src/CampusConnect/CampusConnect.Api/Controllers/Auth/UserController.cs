
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CampusConnect.Application.Interfaces;
using CampusConnect.Application.DTOs;

namespace CampusConnect.Api.Controllers.Auth
{
    [Authorize]
    [Route("api/[controller]")] 
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IActivityLoggerService _activityLogger;

        public UserController(IUserService userService, IActivityLoggerService activityLogger)
        {
            _userService = userService;
            _activityLogger = activityLogger;
        }
        
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        
        var response = users.Select(u => new 
        {
            id = u.Id,
            firstName = u.FirstName,
            lastName = u.LastName,
            profilePictureUrl = u.ProfilePictureUrl,
            studentId = u.StudentId,
            dateofBirth = u.DateOfBirth,

        });

        return Ok(response);
    }   
    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> SearchUsers([FromQuery] string? search)
{
    // 1. Obtinem  lista de useri din baza de date
    var users = await _userService.SearchUsersAsync(search ?? "");
    
    var resultList = new List<UserSummaryDto>();

    // 2. Iterăm prin fiecare user pentru a-i afla rolul specific
    foreach (var user in users)
    {
        // Apelează metoda din service care interoghează Identity
        var roles = await _userService.GetUserRolesAsync(user);

        // 3. Determinăm rolul dominant (Prioritate: Admin > Professor > User)
        string displayRole = "User";

        if (roles.Contains("Admin"))
        {
            displayRole = "Admin";
        }
        else if (roles.Contains("Professor"))
        {
            displayRole = "Professor";
        }

        // 4. Construim obiectul DTO
        resultList.Add(new UserSummaryDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            ProfilePictureUrl = user.ProfilePictureUrl,
            StudentId = user.StudentId,
            Role = displayRole // Aici va fi acum valoarea corectă
        });
    }

    return Ok(resultList);
}

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "ID-ul utilizatorului nu a putut fi extras din token." });
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);

            if (user == null)
            {
                return NotFound(new { message = "Profilul utilizatorului nu a fost gasit." });
            }

            var responseDto = new UserProfileResponse
            {
                Id=user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DateofBirth = user.DateOfBirth, 
                StudentId = user.StudentId
            };
            
            return Ok(responseDto);
        }


        [AllowAnonymous]
        [HttpGet("public-details/{id}")]
        public async Task<IActionResult> GetPublicUserDetails(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                studentId = user.StudentId,
                profilePictureUrl = user.ProfilePictureUrl,
                dateOfBirth = user.DateOfBirth 
            });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "ID-ul utilizatorului nu a putut fi extras din token." });
            }

            var success = await _userService.UpdateUserProfileAsync(userId.Value, model);

            if (success)
            {
                await _activityLogger.LogActivityAsync(userId.Value, "Update", "UserProfile", userId.Value, null, "Updated user profile");
                return Ok(new { message = "Profil actualizat cu succes!" });
            }
            
            return NotFound(new { message = "Eroare. Utilizatorul nu a fost gasit." });
        }

        [HttpDelete("delete/{id?}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "ID-ul utilizatorului nu a putut fi extras din token." });
            }

            int targetId = id ?? currentUserId.Value;
            var isAdmin = User.IsInRole("Admin");

            if (targetId != currentUserId && !isAdmin)
            {
                return Unauthorized(); 
            }

            var success = await _userService.DeleteUserAsync(targetId);
            if (success)
            {
                return NoContent();
            }

            return NotFound(new { message = "Eroare. Utilizatorul nu a fost găsit." });
        }

        [HttpPatch("{id}/toggle-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleAdminRole(int id)
        {
            var newRole = await _userService.ToggleAdminRoleAsync(id);

            if (newRole == null)
            {
                return NotFound(new { message = "Utilizatorul nu a fost găsit." });
            }

            return Ok(new
            {
                message = $"Rolul a fost schimbat cu succes. Noul rol este: {newRole}",
                newRole = newRole
            });
        }

    }
}