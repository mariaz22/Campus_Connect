using CampusConnect.Application.DTOs.RoomBooking;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusConnect.Api.Controllers.Facilities;

[ApiController]
[Route("api/[controller]")]
public class RoomBookingController : ControllerBase
{
    private readonly IRoomBookingService _bookingService;
    private readonly ICurrentUserService _currentUserService;

    public RoomBookingController(
        IRoomBookingService bookingService,
        ICurrentUserService currentUserService)
    {
        _bookingService = bookingService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RoomBookingRequestDto>> CreateBookingRequest(CreateRoomBookingRequest request)
    {
        try
        {
            var userId = _currentUserService.GetCurrentUserId();
            if (userId == null)
                return Unauthorized("User not found");

            var bookingRequest = await _bookingService.CreateBookingRequestAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetRequestById), new { id = bookingRequest.Id }, bookingRequest);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RoomBookingRequestDto>>> GetPendingRequests()
    {
        var requests = await _bookingService.GetPendingRequestsAsync();
        return Ok(requests);
    }

    [HttpGet("my-requests")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RoomBookingRequestDto>>> GetMyRequests()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            return Unauthorized("User not found");

        var requests = await _bookingService.GetMyRequestsAsync(userId.Value);
        return Ok(requests);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RoomBookingRequestDto>> GetRequestById(int id)
    {
        var request = await _bookingService.GetRequestByIdAsync(id);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomBookingRequestDto>> ApproveRequest(int id)
    {
        try
        {
            var adminId = _currentUserService.GetCurrentUserId();
            if (adminId == null)
                return Unauthorized("Admin not found");

            var request = await _bookingService.ApproveRequestAsync(id, adminId.Value);
            return Ok(request);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomBookingRequestDto>> RejectRequest(int id, [FromBody] ReviewBookingRequest review)
    {
        try
        {
            var adminId = _currentUserService.GetCurrentUserId();
            if (adminId == null)
                return Unauthorized("Admin not found");

            var request = await _bookingService.RejectRequestAsync(id, adminId.Value, review.RejectionReason);
            return Ok(request);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        try
        {
            var userId = _currentUserService.GetCurrentUserId();
            if (userId == null)
                return Unauthorized("User not found");

            var result = await _bookingService.DeleteRequestAsync(id, userId.Value);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
