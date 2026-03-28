using CampusConnect.Application.DTOs.CampusMap;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusConnect.Api.Controllers.Facilities;

[ApiController]
[Route("api/[controller]")]
public class RoomReservationController : ControllerBase
{
    private readonly ICampusMapService _campusMapService;

    public RoomReservationController(ICampusMapService campusMapService)
    {
        _campusMapService = campusMapService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoomReservationDto>> CreateReservation(CreateReservationRequest request)
    {
        try
        {
            var reservation = await _campusMapService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(GetUserReservations), reservation);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RoomReservationDto>>> GetPendingReservations()
    {
        var reservations = await _campusMapService.GetPendingReservationsAsync();
        return Ok(reservations);
    }

    [HttpGet("my-reservations")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RoomReservationDto>>> GetUserReservations()
    {
        try
        {
            var reservations = await _campusMapService.GetUserReservationsAsync();
            return Ok(reservations);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPut("{id}/process")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomReservationDto>> ProcessReservation(int id, ProcessReservationRequest request)
    {
        try
        {
            var reservation = await _campusMapService.ProcessReservationAsync(id, request);
            return Ok(reservation);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> CancelReservation(int id)
    {
        try
        {
            var result = await _campusMapService.CancelReservationAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check-availability")]
    [Authorize]
    public async Task<ActionResult<bool>> CheckAvailability(
        [FromQuery] int roomId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        // First check if it's valid booking time
        if (!_campusMapService.IsValidBookingTime(startTime, endTime))
        {
            return Ok(new { available = false, reason = "The reservations can only be made between 8:00 AM and 8:00 PM." });
        }

        var isAvailable = await _campusMapService.IsTimeSlotAvailableAsync(roomId, startTime, endTime);
        return Ok(new { available = isAvailable, reason = isAvailable ? null : "The selected time slot overlaps with an existing reservation." });
    }
}
