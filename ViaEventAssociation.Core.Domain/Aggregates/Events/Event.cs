using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public class Event
{
    public Guid Id { get; } = Guid.NewGuid();

    private string? Title { get; set; }
    private string? Description { get; set; }
    private (DateTime Start, DateTime End) TimeRange { get; set; }
    private string Status { get; set; } = "Draft";
    private string Visibility { get; set; } = "Private";
    private int MaxGuests { get; set; } = 0;

    private List<Guid> GuestList { get; set; } = new List<Guid>();

    private List<Invitation> InvitationList { get; set; } = new List<Invitation>();

    public static Result<Event> CreateEvent()
    {
        // no validation - event needs only an automatically created Guid. It ain't go wrong.
        return Result<Event>.Success(new Event()); // creates an Events object and passing it as a Payload in the returned Result<Events> object.
    }

    public Result<Event> UpdateEventTitle(Guid eventId, string title)
    {
        // TODO: add proper validation based on the use cases / requirements
        if (eventId != Id) return Result<Event>.Failure();

        Title = title;
        // Returns the updated object, so that the application or service layer can f.x. save to DB:  if (updateResult.IsSuccess) repository.Save(updateResult.Payload!);
        return Result<Event>.Success(this);
    }

    public Result<Event> UpdateEventDescription(Guid eventId, string eventDescription)
    {
        // TODO: add proper validation based on the use cases / requirements
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        Description = eventDescription;
        return Result<Event>.Success(this);
    }

    public Result<Event> SetEventTimeRange(Guid eventId, DateTime startTime, DateTime endTime)
    {
        // TODO: add proper validation based on the use cases / requirements
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        TimeRange = (startTime, endTime);
        return Result<Event>.Success(this);
    }

    public Result<Event> SetEventVisibility(Guid eventId)
    {
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        Visibility = "Public";
        return Result<Event>.Success(this);
    }

    public Result<Event> SetMaximumNumberOfGuests(Guid eventId, int maxGuests)
    {
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        MaxGuests = maxGuests;
        return Result<Event>.Success(this);
    }

    public Result<Event> ReadyTheEvent(Guid eventId)
    {
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        Status = "Ready";
        return Result<Event>.Success(this);
    }

    public Result<Event> ActivateTheEvent(Guid eventId)
    {
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        Status = "Active";
        return Result<Event>.Success(this);
    }

    public Result<Event> InviteGuests(Guid eventId, Guid guestId)
    {
        throw new NotImplementedException();
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }
    }

    public Result<Event> AcceptRequest(Guid invitationId)
    {
        throw new NotImplementedException();
    }

    public Result<Event> DeclineRequest(Guid invitationId)
    {
        throw new NotImplementedException();
    }

    public Result<Event> CancelEvent(Guid eventId)
    {
        if (eventId != Id)
        {
            return Result<Event>.Failure();
        }

        Status = "Canceled";
        return Result<Event>.Success(this);
    }
}