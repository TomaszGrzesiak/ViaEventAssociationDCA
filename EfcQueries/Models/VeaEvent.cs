using System;
using System.Collections.Generic;

namespace EfcQueries.Models;

public partial class VeaEvent
{
    public string Id { get; set; } = null!;

    public string TitleValue { get; set; } = null!;

    public string DescriptionValue { get; set; } = null!;

    public string? TimeRangeStartTime { get; set; }

    public string? TimeRangeEndTime { get; set; }

    public int Status { get; set; }

    public int Visibility { get; set; }

    public int LocationMaxCapacity { get; set; }

    public int MaxGuestsNoValue { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
