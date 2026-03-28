using CampusConnect.Api.Controllers.Auth;
using CampusConnect.Application.DTOs.Auth;
using CampusConnect.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CampusConnect.Tests.Controllers.Auth;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    #region Register Tests

    [Fact]
    public async Task Register_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@s.unibuc.ro",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var authResult = new AuthResult
        {
            Success = true,
            Message = "Înregistrare reușită! Verifică email-ul pentru confirmare."
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<AuthResult>().Subject;
        returnedResult.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@gmail.com", // Invalid domain
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Doar email-urile @unibuc.ro sau @s.unibuc.ro sunt permise"
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnedResult = badRequestResult.Value.Should().BeOfType<AuthResult>().Subject;
        returnedResult.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@s.unibuc.ro",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Email-ul este deja înregistrat"
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@s.unibuc.ro",
            Password = "weak", // Weak password
            ConfirmPassword = "weak",
            FirstName = "Test",
            LastName = "User"
        };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Parola trebuie să aibă minim 8 caractere"
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@s.unibuc.ro",
            Password = "Password123!"
        };

        var authResult = new AuthResult
        {
            Success = true,
            Message = "Login successful",
            Data = new LoginResponse
            {
                Token = "jwt-token-here",
                Email = request.Email,
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            }
        };

        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<AuthResult>().Subject;
        returnedResult.Success.Should().BeTrue();
        returnedResult.Data.Should().NotBeNull();
        returnedResult.Data!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@s.unibuc.ro",
            Password = "WrongPassword"
        };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Email sau parolă incorectă"
        };

        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        var returnedResult = unauthorizedResult.Value.Should().BeOfType<AuthResult>().Subject;
        returnedResult.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithUnconfirmedEmail_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "unconfirmed@s.unibuc.ro",
            Password = "Password123!"
        };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Email-ul nu a fost confirmat. Verifică inbox-ul."
        };

        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithLockedAccount_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "locked@s.unibuc.ro",
            Password = "Password123!"
        };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Contul este blocat temporar"
        };

        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region ConfirmEmail Tests

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        var token = "valid-confirmation-token";

        var authResult = new AuthResult
        {
            Success = true,
            Message = "Email confirmat cu succes"
        };

        _mockAuthService
            .Setup(x => x.ConfirmEmailAsync(userId, token))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.ConfirmEmail(userId, token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        var userId = 0; // Invalid
        var token = "some-token";

        // Act
        var result = await _controller.ConfirmEmail(userId, token);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnedResult = badRequestResult.Value.Should().BeOfType<AuthResult>().Subject;
        returnedResult.Message.Should().Be("Parametri invalizi");
    }

    [Fact]
    public async Task ConfirmEmail_WithEmptyToken_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var token = ""; // Empty token

        // Act
        var result = await _controller.ConfirmEmail(userId, token);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ConfirmEmail_WithExpiredToken_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var token = "expired-token";

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Token-ul a expirat"
        };

        _mockAuthService
            .Setup(x => x.ConfirmEmailAsync(userId, token))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.ConfirmEmail(userId, token);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region ResendConfirmation Tests

    [Fact]
    public async Task ResendConfirmation_WithValidEmail_ReturnsOk()
    {
        // Arrange
        var request = new ResendConfirmationRequest { Email = "test@s.unibuc.ro" };

        var authResult = new AuthResult
        {
            Success = true,
            Message = "Email de confirmare retrimis"
        };

        _mockAuthService
            .Setup(x => x.ResendEmailConfirmationAsync(request.Email))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.ResendConfirmation(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ResendConfirmation_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new ResendConfirmationRequest { Email = "" };

        // Act
        var result = await _controller.ResendConfirmation(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnedResult = badRequestResult.Value.Should().BeOfType<AuthResult>().Subject;
        returnedResult.Message.Should().Be("Email obligatoriu");
    }

    [Fact]
    public async Task ResendConfirmation_WithNonExistentEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new ResendConfirmationRequest { Email = "nonexistent@s.unibuc.ro" };

        var authResult = new AuthResult
        {
            Success = false,
            Message = "Email-ul nu există în sistem"
        };

        _mockAuthService
            .Setup(x => x.ResendEmailConfirmationAsync(request.Email))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.ResendConfirmation(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}
