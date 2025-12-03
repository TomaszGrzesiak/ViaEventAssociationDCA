using System;
using System.Collections.Generic;

namespace EfcQueries.Models;

public partial class Invitation
{
    public string InvitationId { get; set; } = null!;

    public string GuestId { get; set; } = null!;

    public int Status { get; set; }

    public string EventId { get; set; } = null!;

    public virtual VeaEvent Event { get; set; } = null!;
}
