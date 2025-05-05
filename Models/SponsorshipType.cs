using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class SponsorshipType
{
    public int SponsorshipId { get; set; }

    public string SponsorshipType1 { get; set; } = null!;

    public virtual ICollection<EventSponsor> EventSponsors { get; set; } = new List<EventSponsor>();

    public virtual ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
}
