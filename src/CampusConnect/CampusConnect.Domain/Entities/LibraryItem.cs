namespace CampusConnect.Domain.Entities;
public class LibraryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FolderId { get; set; }
    public LibraryFolder Folder { get; set; } = default!;

    public string Title { get; set; } = default!;
    public LibraryItemType Type { get; set; }

    public string? Url { get; set; }

    public string? StoredFileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }

    public string CreatedByUserId { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
