using CampusConnect.Application.DTOs.CampusMap;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusConnect.Api.Controllers.Facilities;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ICampusMapService _campusMapService;

    public RoomController(ICampusMapService campusMapService)
    {
        _campusMapService = campusMapService;
    }

    [HttpGet("building/{buildingId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByBuilding(int buildingId)
    {
        var rooms = await _campusMapService.GetRoomsByBuildingAsync(buildingId);
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RoomDetailsDto>> GetRoomDetails(int id)
    {
        var room = await _campusMapService.GetRoomDetailsAsync(id);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("available")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRoomsNow()
    {
        var rooms = await _campusMapService.GetAvailableRoomsNowAsync();
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomRequest request)
    {
        try
        {
            var room = await _campusMapService.CreateRoomAsync(request);
            return CreatedAtAction(nameof(GetRoomDetails), new { id = room.Id }, room);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(int id, UpdateRoomRequest request)
    {
        try
        {
            var room = await _campusMapService.UpdateRoomAsync(id, request);
            return Ok(room);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var result = await _campusMapService.DeleteRoomAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
