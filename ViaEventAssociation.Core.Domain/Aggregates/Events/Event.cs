// using ViaEventAssociation.Core.Tools.OperationResult;
//
// namespace ViaEventAssociation.Core.Domain.Aggregates.Events;
//
// public class Events
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
//     public static Result<Events> CreateEvent()
//     {
//         // no validation - event needs only an automatically created Guid. It ain't go wrong.
//         return Result<Events>.Success(new Events()); // creates an Events object and passing it as a Payload in the returned Result<Events> object.
//     }
//
//     public Result<Events> UpdateEventTitle(Guid eventId, string title)
//     {
//         // TODO: add proper validation based on the use cases / requirements
//         if (eventId != Id) return Result<Events>.Failure("Events ID mismatch.");
//         
//         Title = title;
//         // Returns the updated object, so that the application or service layer can f.x. save to DB:  if (updateResult.IsSuccess) repository.Save(updateResult.Payload!);
//         return Result<Events>.Success(this); 
//     }
//
//     public Result<Events> UpdateEventDescription(Guid eventId, string eventDescription)
//     {
//         // TODO: add proper validation based on the use cases / requirements
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         Description = eventDescription;
//         return Result<Events>.Success(this);
//     }
//
//     public Result<Events> SetEventTimeRange(Guid eventId, DateTime startTime, DateTime endTime)
//     {
//         // TODO: add proper validation based on the use cases / requirements
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         TimeRange = (startTime, endTime);
//         return Result<Events>.Success(this);
//     }
//
//     public Result<Events> SetEventVisibility(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         Visibility = "Public";
//         return Result<Events>.Success(this);
//     }
//
//     public Result<Events> SetMaximumNumberOfGuests(Guid eventId, int maxGuests)
//     {
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         MaxGuests = maxGuests;
//         return Result<Events>.Success(this);
//     }
//
//     public Result<Events> ReadyTheEvent(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         Status = "Ready";
//         return Result<Events>.Success(this);
//     }
//
//     public Result<Events> ActivateTheEvent(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         Status = "Active";
//         return Result<Events>.Success(this);
//     }
//
//     public Result<Events> InviteGuests(Guid eventId, Guid guestId)
//     {
//         throw new NotImplementedException();
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//     }
//
//     public Result<Events> AcceptRequest(Guid invitationId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Result<Events> DeclineRequest(Guid invitationId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Result<Events> CancelEvent(Guid eventId)
//     {
//         if (eventId != Id)
//         {
//             return Result<Events>.Failure("Events ID mismatch.");
//         }
//         Status = "Canceled";
//         return Result<Events>.Success(this);
//     }
// }
