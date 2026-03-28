using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class LibraryItem
{
    public Guid Id { get; set; }

    public Guid FolderId { get; set; }

    public string Title { get; set; } = null!;

    public int Type { get; set; }

    public string? Url { get; set; }

    public string? StoredFileName { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long? SizeBytes { get; set; }

    public string CreatedByUserId { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public virtual LibraryFolder Folder { get; set; } = null!;
}
