using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CampusConnect.Application.DTOs.Auth;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CampusConnect.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly string[] _allowedDomains;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _configuration = configuration;
        _allowedDomains = configuration.GetSection("AllowedEmailDomains").Get<string[]>()
            ?? new[] { "@unibuc.ro", "@s.unibuc.ro" };
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        if (!IsEmailDomainAllowed(request.Email))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Doar studenții sau angajații UniBuc pot crea cont (email-ul trebuie să fie @unibuc.ro sau @s.unibuc.ro)."
            };
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Un cont cu acest email există deja."
            };
        }

        // Determinăm rolul bazat pe email ÎNAINTE de a crea userul
        var normalizedEmail = request.Email.ToLowerInvariant();
        string roleName;

        if (normalizedEmail == "admin1@unibuc.ro" || normalizedEmail == "admin2@unibuc.ro")
        {
            roleName = "Admin";
        }
        else if (normalizedEmail.EndsWith("@s.unibuc.ro"))
        {
            if (normalizedEmail == "anastasia.ispas@s.unibuc.ro" || normalizedEmail == "irina-maria.istrate@s.unibuc.ro")
            {
                roleName = "Professor";
            }
            else
            {
                roleName = "User";
            }
        }
        else if (normalizedEmail.EndsWith("@unibuc.ro"))
        {
            roleName = "Professor";
        }
        else
        {
            roleName = "User";
        }

        // Generăm StudentId automat doar pentru studenți (rol "User")
        string? studentId = null;
        if (roleName == "User")
        {
            studentId = GenerateStudentId();
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            StudentId = studentId,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Eroare la crearea contului. Verifică dacă parola respectă cerințele.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        // Asigură-te că rolul există în bază înainte de a-l atribui
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleName);

        if (!roleResult.Succeeded)
        {
            // Dacă rolul eșuează, ștergem user-ul creat parțial pentru a nu lăsa date inconsistente
            await _userManager.DeleteAsync(user);

            return new AuthResult
            {
                Success = false,
                Message = $"Eroare la atribuirea rolului '{roleName}'. Contul a fost anulat.",
                Errors = roleResult.Errors.Select(e => e.Description).ToList()
            };
        }

        // 6. Generare Token Confirmare Email
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(emailToken);
        var confirmationLink = $"{_configuration["AppSettings:FrontendUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";

        try
        {
            await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);
        }
        catch (Exception ex)
        {
            // Opțional: Logăm eroarea de email dar nu blocăm succesul contului
            Console.WriteLine($"Email Error: {ex.Message}");
        }

        return new AuthResult
        {
            Success = true,
            Message = "Cont creat cu succes! Verifică-ti emailul pentru confirmare."
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthResult { Success = false, Message = "Email sau parolă incorectă." };
        }

        if (!user.EmailConfirmed)
        {
            return new AuthResult { Success = false, Message = "Trebuie să confirmi email-ul înainte de autentificare." };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return new AuthResult { Success = false, Message = "Email sau parolă incorectă." };
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        var token = GenerateJwtToken(user, role);

        var expirationDays = int.Parse(_configuration["JwtSettings:ExpirationInDays"] ?? "7");

        return new AuthResult
        {
            Success = true,
            Message = "Autentificare reușită",
            Data = new LoginResponse
            {
                Token = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
                Role = role,
                StudentId = user.StudentId
            }
        };
    }

    public async Task<AuthResult> ConfirmEmailAsync(int userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new AuthResult { Success = false, Message = "Utilizator invalid." };
        if (user.EmailConfirmed) return new AuthResult { Success = true, Message = "Email-ul tău este deja confirmat. Poți să te autentifici!" };

        var result = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(token));
        if (!result.Succeeded)
        {
            return new AuthResult { Success = false, Message = "Token invalid sau expirat.", Errors = result.Errors.Select(e => e.Description).ToList() };
        }

        return new AuthResult { Success = true, Message = "Email confirmat cu succes!" };
    }

    public async Task<AuthResult> ResendEmailConfirmationAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return new AuthResult { Success = false, Message = "Cont inexistent." };
        if (user.EmailConfirmed) return new AuthResult { Success = false, Message = "Deja confirmat." };

        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_configuration["AppSettings:FrontendUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}";
        await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

        return new AuthResult { Success = true, Message = "Email de confirmare retrimis." };

    }

    private bool IsEmailDomainAllowed(string email)
    {
        return _allowedDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
    }

    private string GenerateJwtToken(ApplicationUser user, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, role)

        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationDays = int.Parse(_configuration["JwtSettings:ExpirationInDays"] ?? "7");
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expirationDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateStudentId()
    {
        // Format: an curent (4 cifre) + 5 cifre random = "202512345"
        var year = DateTime.UtcNow.Year;
        var random = new Random();
        var randomDigits = random.Next(10000, 99999);
        return $"{year}{randomDigits}";
    }
}