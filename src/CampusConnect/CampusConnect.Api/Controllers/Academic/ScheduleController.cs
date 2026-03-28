using CampusConnect.Application.DTOs.CampusMap;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusConnect.Api.Controllers.Academic;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ICampusMapService _campusMapService;

    public ScheduleController(ICampusMapService campusMapService)
    {
        _campusMapService = campusMapService;
    }

    [HttpGet("room/{roomId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetRoomSchedules(
        int roomId,
        [FromQuery] DateTime? date)
    {
        var schedules = await _campusMapService.GetRoomSchedulesAsync(roomId, date);
        return Ok(schedules);
    }

    [HttpGet("room/{roomId}/today")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetRoomSchedulesToday(int roomId)
    {
        var schedules = await _campusMapService.GetRoomSchedulesTodayAsync(roomId);
        return Ok(schedules);
    }

    [HttpPost]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<ScheduleDto>> CreateSchedule(CreateScheduleRequest request)
    {
        try
        {
            var schedule = await _campusMapService.CreateScheduleAsync(request);
            return CreatedAtAction(nameof(GetRoomSchedulesToday), new { roomId = schedule.RoomId }, schedule);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<ScheduleDto>> UpdateSchedule(int id, UpdateScheduleRequest request)
    {
        try
        {
            var schedule = await _campusMapService.UpdateScheduleAsync(id, request);
            return Ok(schedule);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var result = await _campusMapService.DeleteScheduleAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
