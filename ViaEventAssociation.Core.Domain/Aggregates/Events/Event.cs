using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Domain.Events.ValueObjects;
using ViaEventAssociation.Core.Domain.Invitations.ValueObjects;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public sealed class Event : AggregateRoot<EventId>
{
    public EventTitle Title { get; private set; }
    public EventDescription Description { get; private set; }
    public EventTimeRange TimeRange { get; private set; }
    public EventStatus Status { get; private set; }
    public EventVisibility Visibility { get; private set; }
    public MaxGuests MaxGuests { get; private set; }

    private readonly List<Invitation> _invitations = new();
    public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();

    private readonly List<GuestId> _guestList = new();
    public IReadOnlyCollection<GuestId> GuestList => _guestList.AsReadOnly();

    private Event(
        EventId id,
        EventTitle title,
        EventDescription description,
        EventTimeRange timeRange,
        EventVisibility visibility,
        MaxGuests maxGuests)
        : base(id)
    {
        Title = title;
        Description = description;
        TimeRange = timeRange;
        Visibility = visibility;
        MaxGuests = maxGuests;
        Status = EventStatus.Draft;
    }

    // TODO: Add EventVisibility extension of Enumeration.
    // TODO2: Check for other methods based on the UML, because there's lots functionality missing, I believe.
    // TODO: Add more overloads, so that there can be created f.x. an Event with only Status and Description, according to the requirements, etc.
    // TODO: Check if this shit fits all the requirements.
    // TODO: Add Unit test for it

    public static Result<Event> Create(
        EventTitle title,
        EventDescription description,
        EventTimeRange timeRange,
        EventVisibility visibility,
        MaxGuests maxGuests)
    {
        var newEvent = new Event(
            EventId.CreateUnique(),
            title,
            description,
            timeRange,
            visibility,
            maxGuests);

        return Result<Event>.Success(newEvent);
    }

    public Result UpdateTitle(EventTitle newTitle)
    {
        Title = newTitle;
        return Result.Success();
    }

    public Result UpdateDescription(EventDescription newDescription)
    {
        Description = newDescription;
        return Result.Success();
    }

    public Result UpdateTimeRange(EventTimeRange newTimeRange)
    {
        TimeRange = newTimeRange;
        return Result.Success();
    }

    public Result InviteGuest(Invitation invitation)
    {
        if (_invitations.Any(i => i.GuestId == invitation.GuestId))
            return Result.Failure(Error.GuestAlreadyInvited);

        if (_invitations.Count >= MaxGuests.Value)
            return Result.Failure(Error.GuestListFull);

        _invitations.Add(invitation);
        return Result.Success();
    }

    public Result JoinAsGuest(GuestId guestId)
    {
        if (Visibility != EventVisibility.Public)
            return Result.Failure(Error.EventIsNotPublic);

        if (_guestList.Contains(guestId))
            return Result.Failure(Error.GuestAlreadyJoined);

        if (_guestList.Count >= MaxGuests.Value)
            return Result.Failure(Error.GuestListFull);

        _guestList.Add(guestId);
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status.Equals(EventStatus.Active))
            return Result.Failure(Error.EventAlreadyActive);

        Status = EventStatus.Active;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status.Equals(EventStatus.Cancelled))
            return Result.Failure(Error.EventAlreadyCancelled);

        Status = EventStatus.Cancelled;
        return Result.Success();
    }

    public override string ToString() =>
        $"{Title.ToString()} ({Status.ToString()})";
}