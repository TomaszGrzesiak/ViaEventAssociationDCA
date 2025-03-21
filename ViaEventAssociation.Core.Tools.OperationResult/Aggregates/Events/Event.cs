namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Event
{
    public Guid Id { get; } = Guid.NewGuid();
    private string Title { get; set; }
    private string Description { get; set; }
    private (DateTime Start, DateTime End) TimeRange { get; set; }
    private string Status { get; set; } = "Draft";
    private string Visibility { get; set; } = "Private";
    private int MaxGuests { get; set; } = 0;

    public static Result CreateEvent()
    {
        return new Result("Event created successfully.");
    }

    public Result UpdateEventTitle(Guid eventId, string title)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        Title = title;
        return new Result("Event title updated successfully.");
    }

    public Result UpdateEventDescription(Guid eventId, string eventDescription)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        Description = eventDescription;
        return new Result("Event description updated successfully.");
    }

    public Result SetEventTimeRange(Guid eventId, DateTime startTime, DateTime endTime)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        TimeRange = (startTime, endTime);
        return new Result("Event time range set successfully.");
    }

    public Result SetEventVisibility(Guid eventId)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        Visibility = "Public";
        return new Result("Event visibility set to public.");
    }

    public Result SetMaximumNumberOfGuests(Guid eventId, int maxGuests)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        MaxGuests = maxGuests;
        return new Result("Maximum number of guests set successfully.");
    }

    public Result ReadyTheEvent(Guid eventId)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        Status = "Ready";
        return new Result("Event is now ready.");
    }

    public Result ActivateTheEvent(Guid eventId)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        Status = "Active";
        return new Result("Event is now active.");
    }

    public Result InviteGuests(Guid eventId, Guid guestId)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        return new Result("Guest invited successfully.");
    }

    public Result AcceptRequest(Guid invitationId)
    {
        return new Result("Invitation request accepted.");
    }

    public Result DeclineRequest(Guid invitationId)
    {
        return new Result("Invitation request declined.");
    }

    public Result CancelEvent(Guid eventId)
    {
        if (eventId != Id)
        {
            return new Result("Event ID mismatch.");
        }
        Status = "Canceled";
        return new Result("Event has been canceled.");
    }
}
