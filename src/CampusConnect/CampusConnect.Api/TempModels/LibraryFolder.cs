using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class LibraryFolder
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public virtual ICollection<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();
}
