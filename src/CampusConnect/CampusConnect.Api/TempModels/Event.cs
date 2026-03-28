using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public DateTime DateCreated { get; set; }

    public int? OrganizerId { get; set; }

    public string Category { get; set; } = null!;

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual User? Organizer { get; set; }

    public virtual ICollection<SavedEvent> SavedEvents { get; set; } = new List<SavedEvent>();
}
