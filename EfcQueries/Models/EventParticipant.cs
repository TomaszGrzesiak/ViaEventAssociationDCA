using System;
using System.Collections.Generic;

namespace EfcQueries.Models;

public partial class EventParticipant
{
    public string GuestId { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public virtual VeaEvent Event { get; set; } = null!;
}
