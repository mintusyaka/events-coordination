using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class EventVolunteer
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int VolunteerId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Volunteer Volunteer { get; set; } = null!;
}
