using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class PlannedEvent
{
    public int PlannedEventId { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
