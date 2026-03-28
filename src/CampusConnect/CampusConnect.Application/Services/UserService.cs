using CampusConnect.Application.DTOs;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DbContext _dbContext;

        public UserService(UserManager<ApplicationUser> userManager, DbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users.ToList());
        }

        public async Task<IEnumerable<ApplicationUser>> SearchUsersAsync(string search)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                usersQuery = usersQuery.Where(u => u.FirstName.ToLower().Contains(search) 
                                               || u.LastName.ToLower().Contains(search));
            }

            return await Task.FromResult(usersQuery.ToList());
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }
        public async Task<ApplicationUser?> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateUserProfileRequest profileData)
        {
           var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(profileData.DateOfBirth))
            {
                if (DateTime.TryParseExact(profileData.DateOfBirth, "yyyy-MM-dd", 
                               System.Globalization.CultureInfo.InvariantCulture, 
                               System.Globalization.DateTimeStyles.None, 
                               out DateTime dob))
    
               {
                    user.DateOfBirth = dob;
                }
            }
            else
            {
                user.DateOfBirth = null;
            }

            user.FirstName = profileData.FirstName;
            user.LastName = profileData.LastName;
            user.StudentId = profileData.StudentId;
            user.ProfilePictureUrl = profileData.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            // Ștergem toate datele asociate utilizatorului din tabelele cu Restrict/NoAction
            // pentru a evita erori de foreign key constraint

            // EventParticipant (NoAction)
            var eventParticipants = _dbContext.Set<EventParticipant>().Where(ep => ep.UserId == userId);
            _dbContext.Set<EventParticipant>().RemoveRange(eventParticipants);

            // RoomBookingRequest (Restrict) - atât RequestedByUserId cât și ReviewedByAdminId
            var bookingRequests = _dbContext.Set<RoomBookingRequest>().Where(r => r.RequestedByUserId == userId);
            _dbContext.Set<RoomBookingRequest>().RemoveRange(bookingRequests);

            // Setăm ReviewedByAdminId la null pentru cererile revizuite de acest user
            var reviewedRequests = _dbContext.Set<RoomBookingRequest>().Where(r => r.ReviewedByAdminId == userId);
            foreach (var request in reviewedRequests)
            {
                request.ReviewedByAdminId = null;
                request.ReviewedByAdmin = null;
            }

            // Grade (Restrict) - atât StudentId cât și CreatedByProfessorId
            var studentGrades = _dbContext.Set<Grade>().Where(g => g.StudentId == userId);
            _dbContext.Set<Grade>().RemoveRange(studentGrades);

            var professorGrades = _dbContext.Set<Grade>().Where(g => g.CreatedByProfessorId == userId);
            _dbContext.Set<Grade>().RemoveRange(professorGrades);

            // Notification (dacă există Restrict)
            var notifications = _dbContext.Set<Notification>().Where(n => n.UserId == userId);
            _dbContext.Set<Notification>().RemoveRange(notifications);

            // Subject - profesorul asociat (Restrict)
            // Ștergem subiectele create de acest profesor
            var subjects = _dbContext.Set<Subject>().Where(s => s.ProfessorId == userId);
            _dbContext.Set<Subject>().RemoveRange(subjects);

            // Salvăm modificările
            await _dbContext.SaveChangesAsync();

            // Acum putem șterge utilizatorul
            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }
        public async Task<string> ToggleAdminRoleAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null; 

            var currentRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = currentRoles.Contains("Admin");

            string newRole;

            if (isAdmin)
            {
                var email = user.Email?.ToLower() ?? "";

                if (email.Contains("s.unibuc.ro"))
                {
                    newRole = "User"; 
                }
                else if (email.Contains("unibuc.ro"))
                {
                    newRole = "Professor";
                }
                else
                {
                    newRole = "User"; 
                }
            }
            else
            {
                newRole = "Admin";
            }

            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, newRole);
            return newRole;
        }
    }
}