using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class Volunteer
{
    public int VolunteerId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<EventVolunteer> EventVolunteers { get; set; } = new List<EventVolunteer>();

    public virtual User User { get; set; } = null!;
}
