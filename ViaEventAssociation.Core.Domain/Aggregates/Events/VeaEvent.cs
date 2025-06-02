using System.Data;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public sealed class VeaEvent : AggregateRoot<EventId>
{
    public EventTitle Title { get; private set; }
    public EventDescription Description { get; private set; }
    public EventTimeRange? TimeRange { get; private set; }
    public EventStatus Status { get; private set; }
    public EventVisibility Visibility { get; private set; }
    public MaxGuests MaxGuests { get; private set; }

    private readonly List<Invitation> _invitations = new();
    public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();

    private readonly List<GuestId> _guestList = new();
    public IReadOnlyCollection<GuestId> GuestList => _guestList.AsReadOnly();

    public int LocationMaxCapacity { get; private set; }

    private VeaEvent(
        EventId id,
        EventTitle title,
        EventDescription description,
        EventTimeRange? timeRange,
        EventVisibility visibility,
        MaxGuests maxGuests,
        EventStatus? status,
        int? locationMaxCapacity)
        : base(id)
    {
        Title = title;
        Description = description;
        TimeRange = timeRange;
        Visibility = visibility;
        MaxGuests = maxGuests;
        Status = status ?? EventStatus.Draft;
        LocationMaxCapacity = locationMaxCapacity ?? 500;
    }

    public static Result<VeaEvent> Create(
        EventTitle title,
        EventDescription description,
        EventTimeRange timeRange,
        EventVisibility visibility,
        MaxGuests maxGuests,
        int? locationMaxCapacity)
    {
        var newEvent = new VeaEvent(
            EventId.CreateUnique(),
            title,
            description,
            timeRange,
            visibility,
            maxGuests,
            null,
            locationMaxCapacity);

        return Result<VeaEvent>.Success(newEvent);
    }

    public static Result<VeaEvent> Create()
    {
        var newEvent = new VeaEvent(
            EventId.CreateUnique(),
            EventTitle.Default(),
            EventDescription.Default(),
            null,
            EventVisibility.Private,
            MaxGuests.Default(),
            null,
            500);

        return Result<VeaEvent>.Success(newEvent);
    }

    public Result UpdateTitle(EventTitle newTitle)
    {
        if (Status.Equals(EventStatus.Active) || Status.Equals(EventStatus.Cancelled))
            return Result.Failure(Error.ActiveOrCanceledEventCannotBeModified);
        Title = newTitle;
        Status = EventStatus.Draft;
        return Result.Success();
    }

    public Result UpdateDescription(EventDescription newDescription)
    {
        if (Status.Equals(EventStatus.Active) || Status.Equals(EventStatus.Cancelled))
            return Result.Failure(Error.ActiveOrCanceledEventCannotBeModified);
        Description = newDescription;
        Status = EventStatus.Draft;
        return Result.Success();
    }

    public Result UpdateTimeRange(EventTimeRange newTimeRange)
    {
        if (Status.Equals(EventStatus.Active) || Status.Equals(EventStatus.Cancelled))
            return Result.Failure(Error.ActiveOrCanceledEventCannotBeModified);
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
        if (!Status.Equals(EventStatus.Ready))
        {
            var readyResult = Ready();
            if (readyResult.IsFailure)
                return Result.Failure([Error.ActivateFailure, ..readyResult.Errors]);
        }

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

    public Result UpdateMaxGuests(MaxGuests newMaxGuests)
    {
        if (Equals(Status, EventStatus.Active) && newMaxGuests.Value < MaxGuests.Value)
            return Result.Failure([Error.DecreaseMaxGuestsImpossible, Error.EventAlreadyActive]);

        if (Equals(Status, EventStatus.Cancelled))
            return Result.Failure([Error.UpdateMaxGuestsImpossible, Error.EventAlreadyCancelled]);

        if (newMaxGuests.Value > LocationMaxCapacity)
            return Result.Failure([Error.UpdateMaxGuestsImpossible, Error.MaxGuestAboveLocationCapacity]);

        MaxGuests = newMaxGuests;

        if (Equals(Status, EventStatus.Ready))
            Status = EventStatus.Draft;

        return Result.Success();
    }

    public Result UpdateVisibility(EventVisibility newVisibility)
    {
        if (Equals(Status, EventStatus.Cancelled))
            return Result.Failure([Error.UpdateVisibilityImpossible, Error.EventAlreadyCancelled]);

        if (Visibility.Equals(EventVisibility.Public) && newVisibility.Equals(EventVisibility.Private))
            Status = EventStatus.Draft;

        Visibility = newVisibility;


        return Result.Success();
    }

    public Result Ready()
    {
        if (Equals(Status, EventStatus.Active))
            return Result.Failure(Error.EventAlreadyActive);

        if (Equals(Status, EventStatus.Cancelled))
            return Result.Failure(Error.EventAlreadyCancelled);

        if (TimeRange == null)
            return Result.Failure(Error.EventTimeRangeMissing);

        Status = EventStatus.Ready;

        return Result.Success();
    }

    public static Result<VeaEvent> Create(EventTitle title, EventDescription description, EventTimeRange timeRange, EventVisibility visibility,
        MaxGuests maxGuests, EventStatus? status, int locationMaxCapacity)
    {
        var newEvent = new VeaEvent(
            EventId.CreateUnique(),
            title,
            description,
            timeRange,
            visibility,
            maxGuests,
            status,
            locationMaxCapacity);

        return Result<VeaEvent>.Success(newEvent);
    }
}