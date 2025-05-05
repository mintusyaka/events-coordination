using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public DateOnly EndDate { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = null!;

    public int VenueId { get; set; }

    public int OrganizerId { get; set; }

    public virtual ICollection<EventOrganizer> EventOrganizers { get; set; } = new List<EventOrganizer>();

    public virtual ICollection<EventSponsor> EventSponsors { get; set; } = new List<EventSponsor>();

    public virtual ICollection<EventVolunteer> EventVolunteers { get; set; } = new List<EventVolunteer>();

    public virtual Organizer Organizer { get; set; } = null!;

    public virtual ICollection<PlannedEvent> PlannedEvents { get; set; } = new List<PlannedEvent>();

    public virtual Venue Venue { get; set; } = null!;
}
