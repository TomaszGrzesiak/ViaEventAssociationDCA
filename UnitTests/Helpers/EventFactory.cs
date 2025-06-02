using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Helpers;

public class EventFactory
{
    private EventTitle _title = EventTitle.Default();
    private EventDescription _description = EventDescription.Default();
    private EventTimeRange? _timeRange = null;
    private EventVisibility? _visibility = null;
    private MaxGuests _maxGuests = MaxGuests.Create(100).Payload!;
    private EventStatus? _status = EventStatus.Draft;
    private int _locationMaxCapacity = 500;

    public static EventFactory Init() => new();

    public EventFactory WithTitle(string title)
    {
        _title = EventTitle.Create(title).Payload!;
        return this;
    }

    public EventFactory WithDescription(string desc)
    {
        _description = EventDescription.Create(desc).Payload!;
        return this;
    }

    public EventFactory WithVisibility(EventVisibility visibility)
    {
        _visibility = visibility;
        return this;
    }

    public EventFactory WithMaxGuests(int value)
    {
        _maxGuests = MaxGuests.Create(value).Payload!;
        return this;
    }

    public EventFactory WithTimeRange(EventTimeRange timeRange)
    {
        _timeRange = timeRange;
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
        return VeaEvent.Create(_title, _description, _timeRange!, _visibility, _maxGuests, _status, _locationMaxCapacity).Payload!;
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
}