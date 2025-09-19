using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Helpers;

public class EventFactory
{
    private EventId _eventId = EventId.CreateUnique();
    private EventTitle? _title = EventTitle.Default();
    private EventDescription? _description = EventDescription.Default();
    private EventTimeRange? _timeRange = null;
    private EventVisibility? _visibility = null;
    private MaxGuests? _maxGuests = MaxGuests.Create(50).Payload;
    private EventStatus? _status = EventStatus.Draft;
    private int _locationMaxCapacity = 500;
    private List<GuestId> _guests = [];
    private List<Invitation> _invitations = [];

    public static EventFactory Init() => new();

    public EventFactory WithId(EventId eventId)
    {
        _eventId = eventId;
        return this;
    }

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

    public EventFactory WithMaxGuests(int maxGuests)
    {
        _maxGuests = MaxGuests.Create(maxGuests).Payload;
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
        return VeaEvent.Create(_eventId, _title, _description, _timeRange, _visibility, _maxGuests!, _status, _locationMaxCapacity, _guests, _invitations)
            .Payload!;
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

    public EventFactory WithGuest(GuestId guestId)
    {
        _guests.Add(guestId);
        return this;
    }

    public EventFactory WithInvitedGuest(GuestId guestId)
    {
        var invi = Invitation.Create(guestId);
        _invitations.Add(invi);
        return this;
    }
}