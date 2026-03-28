namespace CampusConnect.Domain.Entities;
public class CategorySubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Category { get; set; } = string.Empty;
}
