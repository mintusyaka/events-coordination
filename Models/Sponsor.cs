using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class Sponsor
{
    public int SponsorId { get; set; }

    public int UserId { get; set; }

    public string Organization { get; set; } = null!;

    public virtual ICollection<EventSponsor> EventSponsors { get; set; } = new List<EventSponsor>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<SponsorshipType> Sponsorships { get; set; } = new List<SponsorshipType>();
}
