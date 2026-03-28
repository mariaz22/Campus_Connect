using CampusConnect.Api.Controllers.Social;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace CampusConnect.Tests.Controllers.Social;

public class EventControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IAchievementService> _mockAchievementService;
    private readonly Mock<IActivityLoggerService> _mockActivityLogger;
    private readonly EventController _controller;

    public EventControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockAchievementService = new Mock<IAchievementService>();
        _mockActivityLogger = new Mock<IActivityLoggerService>();
        _controller = new EventController(_context, _mockAchievementService.Object, _mockActivityLogger.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
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

    private async Task<Event> CreateTestEvent(int organizerId, string title = "Test Event")
    {
        var eventItem = new Event
        {
            Title = title,
            Description = "Test Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test",
            OrganizerId = organizerId,
            DateCreated = DateTime.UtcNow
        };
        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();
        return eventItem;
    }

    #region GetUpcomingEvents Tests

    [Fact]
    public async Task GetUpcomingEvents_ReturnsUpcomingEvents()
    {
        // Arrange
        var futureEvent = new Event
        {
            Title = "Future Event",
            Description = "Test",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test",
            OrganizerId = 1,
            DateCreated = DateTime.UtcNow
        };
        var pastEvent = new Event
        {
            Title = "Past Event",
            Description = "Test",
            Date = DateTime.UtcNow.AddDays(-7),
            Category = "Test",
            OrganizerId = 1,
            DateCreated = DateTime.UtcNow
        };

        _context.Events.AddRange(futureEvent, pastEvent);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUpcomingEvents();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var events = okResult.Value.Should().BeAssignableTo<List<Event>>().Subject;
        events.Should().HaveCount(1);
        events.First().Title.Should().Be("Future Event");
    }

    [Fact]
    public async Task GetUpcomingEvents_WithSearchTerm_ReturnsMatchingEvents()
    {
        // Arrange
        var event1 = new Event
        {
            Title = "Programming Workshop",
            Description = "Learn to code",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Tech",
            OrganizerId = 1,
            DateCreated = DateTime.UtcNow
        };
        var event2 = new Event
        {
            Title = "Art Exhibition",
            Description = "Beautiful art",
            Date = DateTime.UtcNow.AddDays(14),
            Category = "Art",
            OrganizerId = 1,
            DateCreated = DateTime.UtcNow
        };

        _context.Events.AddRange(event1, event2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUpcomingEvents("programming");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var events = okResult.Value.Should().BeAssignableTo<List<Event>>().Subject;
        events.Should().HaveCount(1);
        events.First().Title.Should().Be("Programming Workshop");
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsEvent()
    {
        // Arrange
        var eventItem = await CreateTestEvent(1, "Test Event");

        // Act
        var result = await _controller.GetById(eventItem.Id);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEvent = okResult.Value.Should().BeOfType<Event>().Subject;
        returnedEvent.Title.Should().Be("Test Event");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_AsAdmin_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId, "Admin");

        var newEvent = new Event
        {
            Title = "New Event",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test"
        };

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Create", "Event", It.IsAny<int>(), newEvent.Title, "Created a new event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(newEvent);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Create_AsProfessor_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId, "Professor");

        var newEvent = new Event
        {
            Title = "Professor Event",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Academic"
        };

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Create", "Event", It.IsAny<int>(), newEvent.Title, "Created a new event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(newEvent);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Create_AsRegularUser_ReturnsUnauthorized()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId, "User");

        var newEvent = new Event
        {
            Title = "New Event",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test"
        };

        // Act
        var result = await _controller.Create(newEvent);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Create_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        var newEvent = new Event
        {
            Title = "New Event",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test"
        };

        // Act
        var result = await _controller.Create(newEvent);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_AsOrganizer_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId, "Professor");

        var eventItem = await CreateTestEvent(userId);

        var updatedEvent = new Event
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Date = DateTime.UtcNow.AddDays(14),
            Category = "Updated"
        };

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Update", "Event", eventItem.Id, It.IsAny<string>(), "Updated an event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(eventItem.Id, updatedEvent);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        var adminId = 2;
        var organizerId = 1;
        SetupUserContext(adminId, "Admin");

        var eventItem = await CreateTestEvent(organizerId);

        var updatedEvent = new Event
        {
            Title = "Admin Updated",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(14),
            Category = "Admin"
        };

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(adminId, "Update", "Event", eventItem.Id, It.IsAny<string>(), "Updated an event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(eventItem.Id, updatedEvent);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_WithNonExistentEvent_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1, "Admin");

        var updatedEvent = new Event
        {
            Title = "Updated",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test"
        };

        // Act
        var result = await _controller.Update(999, updatedEvent);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ByNonOrganizer_ReturnsUnauthorized()
    {
        // Arrange
        var otherUserId = 2;
        SetupUserContext(otherUserId, "User");

        var eventItem = await CreateTestEvent(1);

        var updatedEvent = new Event
        {
            Title = "Unauthorized Update",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test"
        };

        // Act
        var result = await _controller.Update(eventItem.Id, updatedEvent);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_AsOrganizer_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId, "Professor");

        var eventItem = await CreateTestEvent(userId);

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Delete", "Event", eventItem.Id, eventItem.Title, "Deleted an event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(eventItem.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        var adminId = 2;
        SetupUserContext(adminId, "Admin");

        var eventItem = await CreateTestEvent(1);

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(adminId, "Delete", "Event", eventItem.Id, eventItem.Title, "Deleted an event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(eventItem.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ByNonOrganizer_ReturnsForbid()
    {
        // Arrange
        var otherUserId = 2;
        SetupUserContext(otherUserId, "User");

        var eventItem = await CreateTestEvent(1);

        // Act
        var result = await _controller.Delete(eventItem.Id);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Delete_WithNonExistentEvent_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1, "Admin");

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Participate (Join) Tests

    [Fact]
    public async Task Participate_WithValidEvent_ReturnsOk()
    {
        // Arrange
        var userId = 2;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(1);

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Participate", "Event", eventItem.Id, eventItem.Title, "Participated in an event"))
            .Returns(Task.CompletedTask);

        _mockAchievementService
            .Setup(x => x.CheckAndGrantEventAchievementAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Participate(eventItem.Id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Participate_WhenAlreadyParticipating_ReturnsBadRequest()
    {
        // Arrange
        var userId = 2;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(1);

        // Add existing participation
        _context.EventParticipants.Add(new EventParticipant
        {
            EventId = eventItem.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Participate", "Event", eventItem.Id, eventItem.Title, "Participated in an event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Participate(eventItem.Id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Participate_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        var eventItem = await CreateTestEvent(1);

        // Act
        var result = await _controller.Participate(eventItem.Id);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Participate_WithNonExistentEvent_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1);

        // Act
        var result = await _controller.Participate(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Withdraw (Leave) Tests

    [Fact]
    public async Task Withdraw_WhenParticipating_ReturnsOk()
    {
        // Arrange
        var userId = 2;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(1);

        // Add participation
        _context.EventParticipants.Add(new EventParticipant
        {
            EventId = eventItem.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        _mockActivityLogger
            .Setup(x => x.LogActivityAsync(userId, "Withdraw", "Event", eventItem.Id, eventItem.Title, "Withdrew from an event"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Withdraw(eventItem.Id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Withdraw_WhenNotParticipating_ReturnsBadRequest()
    {
        // Arrange
        var userId = 2;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(1);

        // Act
        var result = await _controller.Withdraw(eventItem.Id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Withdraw_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        var eventItem = await CreateTestEvent(1);

        // Act
        var result = await _controller.Withdraw(eventItem.Id);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region SaveEvent Tests

    [Fact]
    public async Task SaveEvent_WithValidEvent_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(2);

        // Act
        var result = await _controller.SaveEvent(eventItem.Id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SaveEvent_WhenAlreadySaved_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(2);

        // Add existing saved event
        _context.SavedEvents.Add(new SavedEvent
        {
            EventId = eventItem.Id,
            UserId = userId
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.SaveEvent(eventItem.Id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SaveEvent_WithNonExistentEvent_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1);

        // Act
        var result = await _controller.SaveEvent(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region UnsaveEvent Tests

    [Fact]
    public async Task UnsaveEvent_WhenSaved_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(2);

        // Add saved event
        _context.SavedEvents.Add(new SavedEvent
        {
            EventId = eventItem.Id,
            UserId = userId
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.UnsaveEvent(eventItem.Id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task UnsaveEvent_WhenNotSaved_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        var eventItem = await CreateTestEvent(2);

        // Act
        var result = await _controller.UnsaveEvent(eventItem.Id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region GetSavedEvents Tests

    [Fact]
    public async Task GetSavedEvents_ReturnsUserSavedEvents()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        // Create organizer user
        var organizer = new ApplicationUser
        {
            Id = 2,
            FirstName = "Organizer",
            LastName = "Test",
            Email = "org@test.com"
        };
        _context.Users.Add(organizer);

        var event1 = new Event
        {
            Title = "Saved Event 1",
            Description = "Test",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test",
            OrganizerId = 2,
            DateCreated = DateTime.UtcNow
        };
        var event2 = new Event
        {
            Title = "Saved Event 2",
            Description = "Test",
            Date = DateTime.UtcNow.AddDays(14),
            Category = "Test",
            OrganizerId = 2,
            DateCreated = DateTime.UtcNow
        };

        _context.Events.AddRange(event1, event2);
        await _context.SaveChangesAsync();

        _context.SavedEvents.Add(new SavedEvent { EventId = event1.Id, UserId = userId });
        _context.SavedEvents.Add(new SavedEvent { EventId = event2.Id, UserId = userId });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetSavedEvents();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var events = okResult.Value.Should().BeAssignableTo<List<Event>>().Subject;
        events.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetSavedEvents_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        // Act
        var result = await _controller.GetSavedEvents();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion

    #region GetMyParticipatingEvents Tests

    [Fact]
    public async Task GetMyParticipatingEvents_ReturnsUserEvents()
    {
        // Arrange
        var userId = 1;
        SetupUserContext(userId);

        // Create organizer user
        var organizer = new ApplicationUser
        {
            Id = 2,
            FirstName = "Organizer",
            LastName = "Test",
            Email = "org@test.com"
        };
        _context.Users.Add(organizer);

        var eventItem = new Event
        {
            Title = "Participating Event",
            Description = "Test",
            Date = DateTime.UtcNow.AddDays(7),
            Category = "Test",
            OrganizerId = 2,
            DateCreated = DateTime.UtcNow
        };

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        _context.EventParticipants.Add(new EventParticipant
        {
            EventId = eventItem.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetMyParticipatingEvents();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var events = okResult.Value.Should().BeAssignableTo<List<Event>>().Subject;
        events.Should().HaveCount(1);
        events.First().Title.Should().Be("Participating Event");
    }

    [Fact]
    public async Task GetMyParticipatingEvents_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        SetupNoUserContext();

        // Act
        var result = await _controller.GetMyParticipatingEvents();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    #endregion
}
