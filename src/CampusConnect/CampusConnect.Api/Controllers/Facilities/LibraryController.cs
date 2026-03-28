using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using CampusConnect.Infrastructure.Data;
using CampusConnect.Domain.Entities;

namespace CampusConnect.Api.Controllers.Facilities;

[ApiController]
[Route("api/library")]
[Authorize] // orice user logat poate VEDEA
public class LibraryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    private const long MaxFileSizeBytes = 50L * 1024L * 1024L; // 50MB

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "image/png",
        "image/jpeg"
    };

    public LibraryController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ---------------- Helpers ----------------
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            return userId;

        return null;
    }

    private static bool IsValidHttpUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private void TryDeletePhysicalFile(string? storedFileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(storedFileName)) return;
            if (string.IsNullOrWhiteSpace(_env.WebRootPath)) return;

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "library");
            var fullPath = Path.Combine(uploadsDir, storedFileName);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
        catch { }
    }

    // ---------------- DTOs ----------------
    public record CreateFolderRequest(string Name);
    public record CreateLinkRequest(string Title, string Url);

    // =====================================================
    // FOLDERS (READ)
    // =====================================================

    // GET: api/library/folders
    [HttpGet("folders")]
    public async Task<IActionResult> GetFolders()
    {
        var folders = await _context.LibraryFolders
            .OrderByDescending(f => f.CreatedAtUtc)
            .Select(f => new
            {
                f.Id,
                f.Name,
                f.CreatedAtUtc,
                ItemsCount = f.Items.Count
            })
            .ToListAsync();

        return Ok(folders);
    }

    // =====================================================
    // FOLDERS (WRITE) – Admin + Professor
    // =====================================================

    // POST: api/library/folders
    [Authorize(Roles = "Admin,Professor")]
    [HttpPost("folders")]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Folder name is required.");

        var name = request.Name.Trim();

        var exists = await _context.LibraryFolders.AnyAsync(f => f.Name == name);
        if (exists)
            return Conflict("Folder with same name already exists.");

        var folder = new LibraryFolder
        {
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.LibraryFolders.Add(folder);
        await _context.SaveChangesAsync();

        return Ok(new { folder.Id, folder.Name, folder.CreatedAtUtc });
    }

    // DELETE: api/library/folders/{folderId}
    [Authorize(Roles = "Admin,Professor")]
    [HttpDelete("folders/{folderId:guid}")]
    public async Task<IActionResult> DeleteFolder(Guid folderId)
    {
        var folder = await _context.LibraryFolders
            .Include(f => f.Items)
            .FirstOrDefaultAsync(f => f.Id == folderId);

        if (folder == null) return NotFound("Folder not found.");

        foreach (var item in folder.Items.Where(i => i.Type == LibraryItemType.File && !string.IsNullOrWhiteSpace(i.StoredFileName)))
        {
            TryDeletePhysicalFile(item.StoredFileName);
        }

        _context.LibraryFolders.Remove(folder);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Folder deleted successfully" });
    }

    // =====================================================
    // ITEMS (READ)
    // =====================================================

    // GET: api/library/folders/{folderId}/items
    [HttpGet("folders/{folderId:guid}/items")]
    public async Task<IActionResult> GetItems(Guid folderId)
    {
        var exists = await _context.LibraryFolders.AnyAsync(f => f.Id == folderId);
        if (!exists) return NotFound("Folder not found.");

        var items = await _context.LibraryItems
            .Where(i => i.FolderId == folderId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Select(i => new
            {
                i.Id,
                i.Title,
                i.Type,
                i.Url,
                i.OriginalFileName,
                i.ContentType,
                i.SizeBytes,
                i.CreatedAtUtc,
                i.CreatedByUserId
            })
            .ToListAsync();

        return Ok(items);
    }

    // =====================================================
    // ITEMS (WRITE) – Admin + Professor
    // =====================================================

    // POST: api/library/folders/{folderId}/items/link
    [Authorize(Roles = "Admin,Professor")]
    [HttpPost("folders/{folderId:guid}/items/link")]
    public async Task<IActionResult> AddLink(Guid folderId, [FromBody] CreateLinkRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title is required.");
        if (string.IsNullOrWhiteSpace(request.Url)) return BadRequest("Url is required.");

        var folderExists = await _context.LibraryFolders.AnyAsync(f => f.Id == folderId);
        if (!folderExists) return NotFound("Folder not found.");

        var url = request.Url.Trim();
        if (!IsValidHttpUrl(url))
            return BadRequest("Invalid URL. Must start with http:// or https://");

        var item = new LibraryItem
        {
            FolderId = folderId,
            Title = request.Title.Trim(),
            Type = LibraryItemType.Link,
            Url = url,
            CreatedByUserId = userId.Value.ToString(),
            CreatedAtUtc = DateTime.UtcNow,

            StoredFileName = null,
            OriginalFileName = null,
            ContentType = null,
            SizeBytes = null
        };

        _context.LibraryItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { item.Id });
    }

    // POST: api/library/folders/{folderId}/items/file
    [Authorize(Roles = "Admin,Professor")]
    [HttpPost("folders/{folderId:guid}/items/file")]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> UploadFile(Guid folderId, [FromForm] string title, [FromForm] IFormFile file)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title is required.");
        if (file == null || file.Length == 0) return BadRequest("File is required.");

        var folderExists = await _context.LibraryFolders.AnyAsync(f => f.Id == folderId);
        if (!folderExists) return NotFound("Folder not found.");

        if (file.Length > MaxFileSizeBytes)
            return BadRequest("File too large. Max 50MB.");

        if (!string.IsNullOrWhiteSpace(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
            return BadRequest("File type not allowed.");

        if (string.IsNullOrWhiteSpace(_env.WebRootPath))
            return StatusCode(500, "WebRootPath not configured. Ensure wwwroot exists and UseStaticFiles() is enabled.");

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "library");
        Directory.CreateDirectory(uploadsDir);

        var safeOriginal = Path.GetFileName(file.FileName);
        var storedName = $"{Guid.NewGuid():N}_{safeOriginal}";
        var fullPath = Path.Combine(uploadsDir, storedName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var item = new LibraryItem
        {
            FolderId = folderId,
            Title = title.Trim(),
            Type = LibraryItemType.File,
            StoredFileName = storedName,
            OriginalFileName = safeOriginal,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            CreatedByUserId = userId.Value.ToString(),
            CreatedAtUtc = DateTime.UtcNow,

            Url = null
        };

        _context.LibraryItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { item.Id });
    }

    // GET: api/library/items/{itemId}/download
    [HttpGet("items/{itemId:guid}/download")]
    public async Task<IActionResult> Download(Guid itemId)
    {
        var item = await _context.LibraryItems.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return NotFound("Item not found.");
        if (item.Type != LibraryItemType.File) return BadRequest("Item is not a file.");
        if (string.IsNullOrWhiteSpace(item.StoredFileName)) return StatusCode(500, "Missing StoredFileName.");

        if (string.IsNullOrWhiteSpace(_env.WebRootPath))
            return StatusCode(500, "WebRootPath not configured.");

        var fullPath = Path.Combine(_env.WebRootPath, "uploads", "library", item.StoredFileName);
        if (!System.IO.File.Exists(fullPath))
            return NotFound("File not found on disk.");

        return PhysicalFile(fullPath, item.ContentType ?? "application/octet-stream", item.OriginalFileName ?? "download");
    }

    // DELETE: api/library/items/{itemId}
    [Authorize(Roles = "Admin,Professor")]
    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> DeleteItem(Guid itemId)
    {
        var item = await _context.LibraryItems.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return NotFound();

        if (item.Type == LibraryItemType.File && !string.IsNullOrWhiteSpace(item.StoredFileName))
            TryDeletePhysicalFile(item.StoredFileName);

        _context.LibraryItems.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Item deleted successfully" });
    }
}
