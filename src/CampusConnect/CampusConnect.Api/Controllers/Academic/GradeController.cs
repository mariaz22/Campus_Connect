using CampusConnect.Application.DTOs.Grades;
using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusConnect.Api.Controllers.Academic;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradeController(IGradeService gradeService)
    {
        _gradeService = gradeService;
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
    public async Task<ActionResult<GradeDto>> CreateGrade([FromBody] CreateGradeRequest request)
    {
        try
        {
            var professorId = GetCurrentUserId();
            var grade = await _gradeService.CreateGradeAsync(professorId, request);
            return Ok(grade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<GradeDto>> UpdateGrade(int id, [FromBody] UpdateGradeRequest request)
    {
        try
        {
            var professorId = GetCurrentUserId();
            var grade = await _gradeService.UpdateGradeAsync(id, professorId, request);
            return Ok(grade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult> DeleteGrade(int id)
    {
        var professorId = GetCurrentUserId();
        var result = await _gradeService.DeleteGradeAsync(id, professorId);
        
        if (!result)
        {
            return NotFound(new { message = "Grade not found or you don't have permission to delete it." });
        }

        return Ok(new { message = "Grade deleted successfully" });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GradeDto>> GetGrade(int id)
    {
        var grade = await _gradeService.GetGradeByIdAsync(id);
        
        if (grade == null)
        {
            return NotFound(new { message = "Grade not found" });
        }

        return Ok(grade);
    }

    [HttpGet("subject/{subjectId}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<ActionResult<List<GradeDto>>> GetGradesBySubject(int subjectId)
    {
        var grades = await _gradeService.GetGradesBySubjectAsync(subjectId);
        return Ok(grades);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<GradeDto>>> GetGradesByStudent(int studentId)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var isProfessor = User.IsInRole("Professor");

        // Students can only see their own grades, unless they're admin or professor
        if (currentUserId != studentId && !isAdmin && !isProfessor)
        {
            return Forbid();
        }

        var grades = await _gradeService.GetGradesByStudentAsync(studentId);
        return Ok(grades);
    }

    [HttpGet("student/{studentId}/grouped")]
    public async Task<ActionResult<StudentGradesResponse>> GetStudentGradesGrouped(int studentId)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var isProfessor = User.IsInRole("Professor");

        // Students can only see their own grades, unless they're admin or professor
        if (currentUserId != studentId && !isAdmin && !isProfessor)
        {
            return Forbid();
        }

        try
        {
            var gradesResponse = await _gradeService.GetStudentGradesGroupedAsync(studentId);
            return Ok(gradesResponse);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("my-grades")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<StudentGradesResponse>> GetMyGrades()
    {
        try
        {
            var studentId = GetCurrentUserId();
            var gradesResponse = await _gradeService.GetStudentGradesGroupedAsync(studentId);
            return Ok(gradesResponse);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("subject/{subjectId}/student/{studentId}")]
    public async Task<ActionResult<List<GradeDto>>> GetGradesBySubjectAndStudent(int subjectId, int studentId)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var isProfessor = User.IsInRole("Professor");

        // Students can only see their own grades, unless they're admin or professor
        if (currentUserId != studentId && !isAdmin && !isProfessor)
        {
            return Forbid();
        }

        var grades = await _gradeService.GetGradesBySubjectAndStudentAsync(subjectId, studentId);
        return Ok(grades);
    }
}
