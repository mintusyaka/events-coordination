using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class Organizer
{
    public int OrganizerId { get; set; }

    public int UserId { get; set; }

    public string Organization { get; set; } = null!;

    public virtual ICollection<EventOrganizer> EventOrganizers { get; set; } = new List<EventOrganizer>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual User User { get; set; } = null!;
}
