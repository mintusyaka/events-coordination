using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class EventOrganizer
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int OrganizerId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Organizer Organizer { get; set; } = null!;
}
