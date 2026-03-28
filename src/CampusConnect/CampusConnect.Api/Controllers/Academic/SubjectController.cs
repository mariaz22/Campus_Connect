using CampusConnect.Application.DTOs.Grades;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusConnect.Api.Controllers.Academic;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }

    [HttpPost]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<SubjectDto>> CreateSubject([FromBody] CreateSubjectRequest request)
    {
        try
        {
            var professorId = GetCurrentUserId();
            var subject = await _subjectService.CreateSubjectAsync(professorId, request);
            return Ok(subject);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<SubjectDto>> UpdateSubject(int id, [FromBody] UpdateSubjectRequest request)
    {
        try
        {
            var professorId = GetCurrentUserId();
            var subject = await _subjectService.UpdateSubjectAsync(id, professorId, request);
            return Ok(subject);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult> DeleteSubject(int id)
    {
        var professorId = GetCurrentUserId();
        var result = await _subjectService.DeleteSubjectAsync(id, professorId);
        
        if (!result)
        {
            return NotFound(new { message = "Subject not found or you don't have permission to delete it." });
        }

        return Ok(new { message = "Subject deleted successfully" });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectDto>> GetSubject(int id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        
        if (subject == null)
        {
            return NotFound(new { message = "Subject not found" });
        }

        return Ok(subject);
    }

    [HttpGet("my-subjects")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<List<SubjectDto>>> GetMySubjects()
    {
        var professorId = GetCurrentUserId();
        var subjects = await _subjectService.GetSubjectsByProfessorAsync(professorId);
        return Ok(subjects);
    }

    [HttpGet]
    public async Task<ActionResult<List<SubjectDto>>> GetAllSubjects()
    {
        var subjects = await _subjectService.GetAllSubjectsAsync();
        return Ok(subjects);
    }
}
