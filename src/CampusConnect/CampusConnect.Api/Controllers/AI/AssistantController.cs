using CampusConnect.Application.DTOs.Assistant;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusConnect.Api.Controllers.AI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssistantController : ControllerBase
{
    private readonly IAssistantService _assistantService;

    public AssistantController(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _assistantService.ProcessMessageAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("context")]
    public async Task<ActionResult<UserContextDto>> GetUserContext()
    {
        try
        {
            var context = await _assistantService.GetUserContextAsync();
            return Ok(context);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<IEnumerable<string>>> GetSuggestedQuestions()
    {
        try
        {
            var suggestions = await _assistantService.GetSuggestedQuestionsAsync();
            return Ok(suggestions);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
