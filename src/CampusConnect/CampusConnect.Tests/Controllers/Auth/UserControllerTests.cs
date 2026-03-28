using CampusConnect.Api.Controllers.Auth;
using CampusConnect.Application.DTOs;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace CampusConnect.Tests.Controllers.Auth;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IActivityLoggerService> _mockActivityLogger;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockActivityLogger = new Mock<IActivityLoggerService>();
        _controller = new UserController(_mockUserService.Object, _mockActivityLogger.Object);
    }

    private void SetupUserContext(int userId, string role = "User")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    private void SetupNoUserContext()
    {
        var identity = new ClaimsIdentity();
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    #region GetUserDetails Tests

    [Fact]
    public async Task GetUserDetails_WithValidUser_ReturnsOkWithUserProfile()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "Ion",
            LastName = "Popescu",
            ProfilePictureUrl = "https://example.com/pic.jpg",
            DateOfBirth = new DateTime(2000, 1, 15),
            StudentId = "123ABC"
        };

        _mockUserService
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserDetails();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var profile = okResult.Value.Should().BeOfType<UserProfileResponse>().Subject;
        profile.FirstName.Should().Be("Ion");
        profile.LastName.Should().Be("Popescu");
    }

    [Fact]
    public async Task GetUserDetails_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        // Act
        var result = await _controller.GetUserDetails();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetUserDetails_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        SetupUserContext(userId);

        _mockUserService
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _controller.GetUserDetails();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetPublicUserDetails Tests

    [Fact]
    public async Task GetPublicUserDetails_WithValidId_ReturnsOkWithUserData()
    {
        // Arrange
        var userId = 1;
        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "Maria",
            LastName = "Ionescu",
            StudentId = "456DEF",
            ProfilePictureUrl = "https://example.com/maria.jpg",
            DateOfBirth = new DateTime(1999, 5, 20)
        };

        _mockUserService
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetPublicUserDetails(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPublicUserDetails_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;

        _mockUserService
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _controller.GetPublicUserDetails(userId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "Ion Updated",
            LastName = "Popescu Updated"
        };

        _mockUserService
            .Setup(x => x.UpdateUserProfileAsync(userId, updateRequest))
            .ReturnsAsync(true);

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Update", "UserProfile", userId, null, "Updated user profile"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateUser(updateRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await _controller.UpdateUser(updateRequest);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        SetupUserContext(userId);

        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "Test",
            LastName = "User"
        };

        _mockUserService
            .Setup(x => x.UpdateUserProfileAsync(userId, updateRequest))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateUser(updateRequest);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public async Task DeleteUser_OwnAccount_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        _mockUserService
            .Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(null);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUser_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        var adminId = 1;
        var targetUserId = 2;
        SetupUserContext(adminId, "Admin");

        _mockUserService
            .Setup(x => x.DeleteUserAsync(targetUserId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(targetUserId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUser_OtherUserWithoutAdmin_ReturnsUnauthorized()
    {
        // Arrange
        var userId = 1;
        var targetUserId = 2;
        SetupUserContext(userId, "User");

        // Act
        var result = await _controller.DeleteUser(targetUserId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task DeleteUser_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task DeleteUser_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        _mockUserService
            .Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(null);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region ToggleAdminRole Tests

    [Fact]
    public async Task ToggleAdminRole_AsAdmin_ReturnsOkWithNewRole()
    {
        // Arrange
        var adminId = 1;
        var targetUserId = 2;
        SetupUserContext(adminId, "Admin");

        _mockUserService
            .Setup(x => x.ToggleAdminRoleAsync(targetUserId))
            .ReturnsAsync("Admin");

        // Act
        var result = await _controller.ToggleAdminRole(targetUserId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ToggleAdminRole_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var adminId = 1;
        var targetUserId = 999;
        SetupUserContext(adminId, "Admin");

        _mockUserService
            .Setup(x => x.ToggleAdminRoleAsync(targetUserId))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _controller.ToggleAdminRole(targetUserId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region SearchUsers Tests

    [Fact]
    public async Task SearchUsers_WithSearchTerm_ReturnsMatchingUsers()
    {
        // Arrange
        SetupUserContext(1);
        var searchTerm = "Ion";

        var users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, FirstName = "Ion", LastName = "Popescu", Email = "ion@test.com" },
            new ApplicationUser { Id = 2, FirstName = "Ionela", LastName = "Vasile", Email = "ionela@test.com" }
        };

        _mockUserService
            .Setup(x => x.SearchUsersAsync(searchTerm))
            .ReturnsAsync(users);

        _mockUserService
            .Setup(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _controller.SearchUsers(searchTerm);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUsers = okResult.Value.Should().BeAssignableTo<List<UserSummaryDto>>().Subject;
        returnedUsers.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchUsers_WithEmptySearch_ReturnsAllUsers()
    {
        // Arrange
        SetupUserContext(1);

        var users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, FirstName = "Ion", LastName = "Popescu", Email = "ion@test.com" },
            new ApplicationUser { Id = 2, FirstName = "Maria", LastName = "Ionescu", Email = "maria@test.com" }
        };

        _mockUserService
            .Setup(x => x.SearchUsersAsync(""))
            .ReturnsAsync(users);

        _mockUserService
            .Setup(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _controller.SearchUsers(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUsers = okResult.Value.Should().BeAssignableTo<List<UserSummaryDto>>().Subject;
        returnedUsers.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchUsers_WithAdminRole_ReturnsCorrectRole()
    {
        // Arrange
        SetupUserContext(1);
        var searchTerm = "Admin";

        var users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1, FirstName = "Admin", LastName = "User", Email = "admin@test.com" }
        };

        _mockUserService
            .Setup(x => x.SearchUsersAsync(searchTerm))
            .ReturnsAsync(users);

        _mockUserService
            .Setup(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "Admin", "User" });

        // Act
        var result = await _controller.SearchUsers(searchTerm);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUsers = okResult.Value.Should().BeAssignableTo<List<UserSummaryDto>>().Subject;
        returnedUsers.First().Role.Should().Be("Admin");
    }

    #endregion
}
