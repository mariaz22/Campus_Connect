namespace CampusConnect.Domain.Entities;

public class GroupAnnouncement
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    
    public int AnnouncementId { get; set; }
    public Announcement Announcement { get; set; } = null!;
    
    public int ForwardedByProfessorId { get; set; }
    public ApplicationUser ForwardedByProfessor { get; set; } = null!;
    
    public DateTime ForwardedAt { get; set; }
}
