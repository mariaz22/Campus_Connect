using CampusConnect.Application.DTOs.CampusMap;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusConnect.Api.Controllers.Facilities;

[ApiController]
[Route("api/[controller]")]
public class BuildingController : ControllerBase
{
    private readonly ICampusMapService _campusMapService;

    public BuildingController(ICampusMapService campusMapService)
    {
        _campusMapService = campusMapService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<BuildingDto>>> GetAllBuildings()
    {
        var buildings = await _campusMapService.GetAllBuildingsAsync();
        return Ok(buildings);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<BuildingDto>> GetBuildingById(int id)
    {
        var building = await _campusMapService.GetBuildingByIdAsync(id);
        if (building == null)
            return NotFound();

        return Ok(building);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BuildingDto>> CreateBuilding(CreateBuildingRequest request)
    {
        var building = await _campusMapService.CreateBuildingAsync(request);
        return CreatedAtAction(nameof(GetBuildingById), new { id = building.Id }, building);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BuildingDto>> UpdateBuilding(int id, UpdateBuildingRequest request)
    {
        try
        {
            var building = await _campusMapService.UpdateBuildingAsync(id, request);
            return Ok(building);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBuilding(int id)
    {
        var result = await _campusMapService.DeleteBuildingAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
