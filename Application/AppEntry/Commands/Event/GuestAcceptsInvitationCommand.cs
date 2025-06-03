using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class GuestAcceptsInvitationCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }
    public InvitationId InvitationId { get; }
    
    private GuestAcceptsInvitationCommand(EventId eventId, GuestId guestId, InvitationId invitationId)
    {
        EventId = eventId;
        GuestId = guestId;
        InvitationId = invitationId;
    }

    public static Result<GuestAcceptsInvitationCommand> Create(string eventId, string guestId, string invitationId)
    {
        var eventResult = EventId.FromString(eventId);
        var guestResult = GuestId.FromString(guestId);
        var invitationResult = InvitationId.FromString(invitationId);
        
        var errors = new List<Error>();
        
        if (!eventResult.IsSuccess)
            errors.AddRange(eventResult.Errors);
            
        if (!guestResult.IsSuccess)
            errors.AddRange(guestResult.Errors);
            
        if (!invitationResult.IsSuccess)
            errors.AddRange(invitationResult.Errors);
        
        // if (errors.Any())
        // {
        //     return Result<GuestAcceptsInvitationCommand>.Failure(errors);
        // }
        if (errors.Any())
        {
            return Result<GuestAcceptsInvitationCommand>.Failure(errors.GetEnumerator().Current);
        }
        
        return Result<GuestAcceptsInvitationCommand>.Success(
            new GuestAcceptsInvitationCommand(
                eventResult.Payload, 
                guestResult.Payload, 
                invitationResult.Payload));
    }
}