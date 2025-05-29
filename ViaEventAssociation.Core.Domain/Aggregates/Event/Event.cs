// using ViaEventAssociation.Core.Tools.OperationResult;
//
// namespace ViaEventAssociation.Core.Domain.Aggregates.Event;
//
// public class Event
// {
//     // TODO: make Value objects according to the domain model and assign them to the properties below: such like EventTitle, EventDescription, MaxGuests etc.
//     /** Why is it worth to make EventId as value object, even with just a Value of type Guid?
//      *
//      * Prevents mix-ups like this:
//      * AcceptInvitation(guestId: Guid, invitationId: Guid); // ⚠️ silently wrong - easy to make a mistake and mix when other layers use it and possibly mix which id goes first
//      *
//      * With value objects:
//      * AcceptInvitation(GuestId guestId, InvitationId invitationId); // ❌ won't compile if a programmer mismatch their implementation
//      */
//     private Guid Id { get; } = Guid.NewGuid();
//     private string? Title { get; set; }
//     private string? Description { get; set; }
//     private (DateTime Start, DateTime End) TimeRange{ get; set; }
//     private string Status { get; set; } = "Draft";
//     private string Visibility { get; set; } = "Private";
//     private int MaxGuests { get; set; } = 0;
//     
//     private List<Guid> GuestList { get; set; } = new List<Guid>();
//
//     private List<Invitation> InvitationList { get; set; } = new List<Invitation>();
//     
//     public static Result<Event> CreateEvent()
//     {
//         // no validation - event needs only an automatically created Guid. It ain't go wrong.
//         return Result<Event>.Success(new Event()); // creates an Event object and passing it as a Payload in the returned Result<Event> object.
//     }
//
//     public Result<Event> UpdateEventTitle(Guid eventId, string title)
//     {
//         // TODO: add proper validation based on the use cases / requirements
//         if (eventId != Id) return Result<Event>.Failure("Event ID mismatch.");
//         
//         Title = title;
//         // Returns the updated object, so that the application or service layer can f.x. save to DB:  if (updateResult.IsSuccess) repository.Save(updateResult.Payload!);
//         return Result<Event>.Success(this); 
//     }
//
//     public Result<Event> UpdateEventDescription(Guid eventId, string eventDescription)
//     {
//         // TODO: add proper validation based on the use cases / requirements
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         Description = eventDescription;
//         return Result<Event>.Success(this);
//     }
//
//     public Result<Event> SetEventTimeRange(Guid eventId, DateTime startTime, DateTime endTime)
//     {
//         // TODO: add proper validation based on the use cases / requirements
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         TimeRange = (startTime, endTime);
//         return Result<Event>.Success(this);
//     }
//
//     public Result<Event> SetEventVisibility(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         Visibility = "Public";
//         return Result<Event>.Success(this);
//     }
//
//     public Result<Event> SetMaximumNumberOfGuests(Guid eventId, int maxGuests)
//     {
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         MaxGuests = maxGuests;
//         return Result<Event>.Success(this);
//     }
//
//     public Result<Event> ReadyTheEvent(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         Status = "Ready";
//         return Result<Event>.Success(this);
//     }
//
//     public Result<Event> ActivateTheEvent(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         Status = "Active";
//         return Result<Event>.Success(this);
//     }
//
//     public Result<Event> InviteGuests(Guid eventId, Guid guestId)
//     {
//         throw new NotImplementedException();
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//     }
//
//     public Result<Event> AcceptRequest(Guid invitationId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Result<Event> DeclineRequest(Guid invitationId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Result<Event> CancelEvent(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Event>.Failure("Event ID mismatch.");
//         }
//         Status = "Canceled";
//         return Result<Event>.Success(this);
//     }
// }
