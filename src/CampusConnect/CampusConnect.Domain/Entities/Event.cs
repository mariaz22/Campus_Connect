using CampusConnect.Domain.Entities;
using System;
namespace CampusConnect.Domain.Entities;
public class Event
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public DateTime DateCreated { get; set; }  = DateTime.Now;
    public int? OrganizerId { get; set; } 
    public string Category { get; set; }
    public ApplicationUser? Organizer { get; set; }
    public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();

}