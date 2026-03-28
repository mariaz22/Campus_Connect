namespace CampusConnect.Domain.Entities;

public class LibraryFolder
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<LibraryItem> Items { get; set; } = new List<LibraryItem>();
}
