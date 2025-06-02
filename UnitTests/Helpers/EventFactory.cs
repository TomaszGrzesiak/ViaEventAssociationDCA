using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Helpers;

public class EventFactory
{
    private EventTitle? _title = EventTitle.Default();
    private EventDescription? _description = EventDescription.Default();
    private EventTimeRange? _timeRange = EventTimeRange.Default();
    private EventVisibility? _visibility = null;
    private MaxGuests? _maxGuests = MaxGuests.Create(50);
    private EventStatus? _status = EventStatus.Draft;
    private int _locationMaxCapacity = 500;
    private List<GuestId> _guests = [];

    public static EventFactory Init() => new();

    public EventFactory WithTitle(string? title)
    {
        if (title != null) _title = EventTitle.Create(title).Payload!;
        else _title = null;
        return this;
    }

    public EventFactory WithDescription(string? desc)
    {
        _description = desc != null ? EventDescription.Create(desc).Payload! : null;
        return this;
    }

    public EventFactory WithVisibility(EventVisibility visibility)
    {
        _visibility = visibility;
        return this;
    }

    public EventFactory WithMaxGuests(int value)
    {
        _maxGuests = MaxGuests.Create(value);
        return this;
    }

    public EventFactory WithTimeRange(EventTimeRange? timeRange)
    {
        _timeRange = timeRange != null ? timeRange : null;
        return this;
    }

    public EventFactory WithStatus(EventStatus status)
    {
        _status = status;
        return this;
    }

    public EventFactory WithLocationMaxCapacity(int value)
    {
        _locationMaxCapacity = value;
        return this;
    }

    public VeaEvent Build()
    {
        return VeaEvent.Create(_title, _description, _timeRange!, _visibility, _maxGuests!, _status, _locationMaxCapacity, _guests).Payload!;
    }

    public EventFactory WithValidTitle()
    {
        _title = EventTitle.Create("Valid title.").Payload!;
        return this;
    }

    public EventFactory WithValidDescription()
    {
        _description = EventDescription.Create("Valid description").Payload!;
        return this;
    }

    public EventFactory WithGuest(GuestId guestIds)
    {
        _guests.Add(guestIds);
        return this;
    }

    public EventFactory WithInvitation(GuestId guestIds)
    {
        _guests.Add(guestIds);
        return this;
    }
}