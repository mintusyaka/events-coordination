using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal Balance { get; set; }

    public virtual ICollection<Organizer> Organizers { get; set; } = new List<Organizer>();

    public virtual ICollection<Payment> PaymentPayers { get; set; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentRecipients { get; set; } = new List<Payment>();

    public virtual ICollection<PlannedEvent> PlannedEvents { get; set; } = new List<PlannedEvent>();

    public virtual ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();

    public virtual ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
}
