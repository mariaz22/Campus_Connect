using CampusConnect.Api.Controllers.Facilities;
using CampusConnect.Application.DTOs.RoomBooking;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CampusConnect.Tests.Controllers.Facilities;

public class RoomBookingControllerTests
{
    private readonly Mock<IRoomBookingService> _mockBookingService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly RoomBookingController _controller;

    public RoomBookingControllerTests()
    {
        _mockBookingService = new Mock<IRoomBookingService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _controller = new RoomBookingController(
            _mockBookingService.Object,
            _mockCurrentUserService.Object);
    }

    #region CreateBookingRequest Tests

    [Fact]
    public async Task CreateBookingRequest_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var request = new CreateRoomBookingRequest
        {
            Title = "Ședință de proiect",
            RoomId = 1,
            StartTime = DateTime.Now.AddDays(1).AddHours(10),
            EndTime = DateTime.Now.AddDays(1).AddHours(12)
        };

        var expectedBooking = new RoomBookingRequestDto
        {
            Id = 1,
            Title = request.Title,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RequestedByUserId = userId,
            Status = BookingRequestStatus.Pending
        };

        _mockBookingService
            .Setup(x => x.CreateBookingRequestAsync(request, userId))
            .ReturnsAsync(expectedBooking);

        // Act
        var result = await _controller.CreateBookingRequest(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedBooking = createdResult.Value.Should().BeOfType<RoomBookingRequestDto>().Subject;
        returnedBooking.Title.Should().Be("Ședință de proiect");
        returnedBooking.Status.Should().Be(BookingRequestStatus.Pending);
    }

    [Fact]
    public async Task CreateBookingRequest_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((int?)null);

        var request = new CreateRoomBookingRequest
        {
            Title = "Test",
            RoomId = 1,
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(2)
        };

        // Act
        var result = await _controller.CreateBookingRequest(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task CreateBookingRequest_WithConflict_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var request = new CreateRoomBookingRequest
        {
            Title = "Test",
            RoomId = 1,
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(2)
        };

        _mockBookingService
            .Setup(x => x.CreateBookingRequestAsync(request, userId))
            .ThrowsAsync(new Exception("Sala este deja rezervată în acest interval"));

        // Act
        var result = await _controller.CreateBookingRequest(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateBookingRequest_WithPastDate_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var request = new CreateRoomBookingRequest
        {
            Title = "Test",
            RoomId = 1,
            StartTime = DateTime.Now.AddDays(-1), // Data în trecut
            EndTime = DateTime.Now.AddDays(-1).AddHours(2)
        };

        _mockBookingService
            .Setup(x => x.CreateBookingRequestAsync(request, userId))
            .ThrowsAsync(new Exception("Nu se pot face rezervări în trecut"));

        // Act
        var result = await _controller.CreateBookingRequest(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region GetPendingRequests Tests

    [Fact]
    public async Task GetPendingRequests_ReturnsOkWithPendingRequests()
    {
        // Arrange
        var pendingRequests = new List<RoomBookingRequestDto>
        {
            new RoomBookingRequestDto
            {
                Id = 1,
                Title = "Request 1",
                Status = BookingRequestStatus.Pending
            },
            new RoomBookingRequestDto
            {
                Id = 2,
                Title = "Request 2",
                Status = BookingRequestStatus.Pending
            }
        };

        _mockBookingService
            .Setup(x => x.GetPendingRequestsAsync())
            .ReturnsAsync(pendingRequests);

        // Act
        var result = await _controller.GetPendingRequests();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRequests = okResult.Value.Should().BeAssignableTo<IEnumerable<RoomBookingRequestDto>>().Subject;
        returnedRequests.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPendingRequests_WhenNoPending_ReturnsEmptyList()
    {
        // Arrange
        _mockBookingService
            .Setup(x => x.GetPendingRequestsAsync())
            .ReturnsAsync(new List<RoomBookingRequestDto>());

        // Act
        var result = await _controller.GetPendingRequests();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRequests = okResult.Value.Should().BeAssignableTo<IEnumerable<RoomBookingRequestDto>>().Subject;
        returnedRequests.Should().BeEmpty();
    }

    #endregion

    #region GetMyRequests Tests

    [Fact]
    public async Task GetMyRequests_ReturnsUserRequests()
    {
        // Arrange
        var userId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var userRequests = new List<RoomBookingRequestDto>
        {
            new RoomBookingRequestDto
            {
                Id = 1,
                Title = "My Request",
                RequestedByUserId = userId,
                Status = BookingRequestStatus.Approved
            }
        };

        _mockBookingService
            .Setup(x => x.GetMyRequestsAsync(userId))
            .ReturnsAsync(userRequests);

        // Act
        var result = await _controller.GetMyRequests();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRequests = okResult.Value.Should().BeAssignableTo<IEnumerable<RoomBookingRequestDto>>().Subject;
        returnedRequests.Should().HaveCount(1);
        returnedRequests.First().RequestedByUserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetMyRequests_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((int?)null);

        // Act
        var result = await _controller.GetMyRequests();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region GetRequestById Tests

    [Fact]
    public async Task GetRequestById_WithValidId_ReturnsOk()
    {
        // Arrange
        var requestId = 1;
        var bookingRequest = new RoomBookingRequestDto
        {
            Id = requestId,
            Title = "Test Request",
            RoomName = "Sala 101"
        };

        _mockBookingService
            .Setup(x => x.GetRequestByIdAsync(requestId))
            .ReturnsAsync(bookingRequest);

        // Act
        var result = await _controller.GetRequestById(requestId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRequest = okResult.Value.Should().BeOfType<RoomBookingRequestDto>().Subject;
        returnedRequest.Id.Should().Be(requestId);
    }

    [Fact]
    public async Task GetRequestById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var requestId = 999;

        _mockBookingService
            .Setup(x => x.GetRequestByIdAsync(requestId))
            .ReturnsAsync((RoomBookingRequestDto?)null);

        // Act
        var result = await _controller.GetRequestById(requestId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region ApproveRequest Tests

    [Fact]
    public async Task ApproveRequest_WithValidId_ReturnsOkWithApprovedRequest()
    {
        // Arrange
        var adminId = 1;
        var requestId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(adminId);

        var approvedRequest = new RoomBookingRequestDto
        {
            Id = requestId,
            Title = "Approved Request",
            Status = BookingRequestStatus.Approved,
            ReviewedByAdminId = adminId,
            ReviewedAt = DateTime.UtcNow
        };

        _mockBookingService
            .Setup(x => x.ApproveRequestAsync(requestId, adminId))
            .ReturnsAsync(approvedRequest);

        // Act
        var result = await _controller.ApproveRequest(requestId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRequest = okResult.Value.Should().BeOfType<RoomBookingRequestDto>().Subject;
        returnedRequest.Status.Should().Be(BookingRequestStatus.Approved);
        returnedRequest.ReviewedByAdminId.Should().Be(adminId);
    }

    [Fact]
    public async Task ApproveRequest_WithNoAdmin_ReturnsUnauthorized()
    {
        // Arrange
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((int?)null);

        // Act
        var result = await _controller.ApproveRequest(1);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task ApproveRequest_WithAlreadyApproved_ReturnsBadRequest()
    {
        // Arrange
        var adminId = 1;
        var requestId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(adminId);

        _mockBookingService
            .Setup(x => x.ApproveRequestAsync(requestId, adminId))
            .ThrowsAsync(new Exception("Cererea a fost deja procesată"));

        // Act
        var result = await _controller.ApproveRequest(requestId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region RejectRequest Tests

    [Fact]
    public async Task RejectRequest_WithValidIdAndReason_ReturnsOkWithRejectedRequest()
    {
        // Arrange
        var adminId = 1;
        var requestId = 1;
        var rejectionReason = "Sala nu este disponibilă în acea perioadă";
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(adminId);

        var review = new ReviewBookingRequest { RejectionReason = rejectionReason };

        var rejectedRequest = new RoomBookingRequestDto
        {
            Id = requestId,
            Title = "Rejected Request",
            Status = BookingRequestStatus.Rejected,
            ReviewedByAdminId = adminId,
            RejectionReason = rejectionReason
        };

        _mockBookingService
            .Setup(x => x.RejectRequestAsync(requestId, adminId, rejectionReason))
            .ReturnsAsync(rejectedRequest);

        // Act
        var result = await _controller.RejectRequest(requestId, review);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRequest = okResult.Value.Should().BeOfType<RoomBookingRequestDto>().Subject;
        returnedRequest.Status.Should().Be(BookingRequestStatus.Rejected);
        returnedRequest.RejectionReason.Should().Be(rejectionReason);
    }

    [Fact]
    public async Task RejectRequest_WithNoAdmin_ReturnsUnauthorized()
    {
        // Arrange
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((int?)null);

        // Act
        var result = await _controller.RejectRequest(1, new ReviewBookingRequest { RejectionReason = "Test" });

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region DeleteRequest Tests

    [Fact]
    public async Task DeleteRequest_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        var requestId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        _mockBookingService
            .Setup(x => x.DeleteRequestAsync(requestId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteRequest(requestId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteRequest_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var requestId = 999;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        _mockBookingService
            .Setup(x => x.DeleteRequestAsync(requestId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteRequest(requestId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteRequest_WithNoUser_ReturnsUnauthorized()
    {
        // Arrange
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((int?)null);

        // Act
        var result = await _controller.DeleteRequest(1);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task DeleteRequest_WithOtherUsersRequest_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var requestId = 1;
        _mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        _mockBookingService
            .Setup(x => x.DeleteRequestAsync(requestId, userId))
            .ThrowsAsync(new Exception("Nu aveți permisiunea să ștergeți această cerere"));

        // Act
        var result = await _controller.DeleteRequest(requestId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}
