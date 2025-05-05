using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public int Capacity { get; set; }

    public string VenueType { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
