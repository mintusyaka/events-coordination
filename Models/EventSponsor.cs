using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class EventSponsor
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int SponsorId { get; set; }

    public int SponsorshipId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Sponsor Sponsor { get; set; } = null!;

    public virtual SponsorshipType Sponsorship { get; set; } = null!;
}
