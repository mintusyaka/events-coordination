using System;
using System.Collections.Generic;

namespace events_coordination_frontend.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly PaymentDate { get; set; }

    public TimeOnly PaymentTime { get; set; }

    public int PayerId { get; set; }

    public int RecipientId { get; set; }

    public virtual User Payer { get; set; } = null!;

    public virtual User Recipient { get; set; } = null!;
}
