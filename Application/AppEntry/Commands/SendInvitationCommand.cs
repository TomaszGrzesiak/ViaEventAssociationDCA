using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.CommandDispatching.Creator;

public class SendInvitationCommand
{
    public int GuestId { get; }
    public int EventId { get; }
    
    public static Result<SendInvitationCommand> Create(int guestId, int eventId)
    {
    
        if(guestId <= 0)
            return Result<SendInvitationCommand>.Failure(Error.InvalidGuestId);

        if (eventId <= 0)
            return Result<SendInvitationCommand>.Failure(Error.InvalidEventId);

        return Result<SendInvitationCommand>.Success(new SendInvitationCommand(guestId, eventId));
    }

    private SendInvitationCommand(int guestId, int eventId)
        => (GuestId, EventId) = (guestId, eventId);
}