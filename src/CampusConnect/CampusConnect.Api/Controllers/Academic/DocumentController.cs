using CampusConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusConnect.Api.Controllers.Academic;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
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

    [HttpGet("enrollment-certificate")]
    public async Task<IActionResult> GetEnrollmentCertificate()
    {
        try
        {
            var userId = GetCurrentUserId();
            var pdfBytes = await _documentService.GenerateEnrollmentCertificateAsync(userId);

            return File(pdfBytes, "application/pdf", $"Adeverinta_Student_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("transcript")]
    public async Task<IActionResult> GetTranscript()
    {
        try
        {
            var userId = GetCurrentUserId();
            var pdfBytes = await _documentService.GenerateTranscriptAsync(userId);

            return File(pdfBytes, "application/pdf", $"Situatie_Scolara_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}