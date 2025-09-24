using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;

public class RegisterGuestCommand
{
    public GuestId GuestId { get; }
    public EmailAddress EmailAddress { get; }
    public GuestName FirstName { get; }
    public GuestName LastName { get; }
    public ProfilePictureUrl ProfilePictureUrlAddress { get; }

    private RegisterGuestCommand(GuestId guestId, EmailAddress emailAddress, GuestName firstName, GuestName lastName,
        ProfilePictureUrl profilePictureUrlAddress)
    {
        GuestId = guestId;
        EmailAddress = emailAddress;
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrlAddress = profilePictureUrlAddress;
    }


    public static Result<RegisterGuestCommand> Create(string guid, string emailAddress, string firstName, string lastName, string pictureUrl)
    {
        var errors = new List<Error>();

        var rGuestId = GuestId.FromString(guid);
        if (rGuestId.IsFailure) errors.AddRange(rGuestId.Errors);

        var rEmailAddress = EmailAddress.Create(emailAddress);
        if (rEmailAddress.IsFailure) errors.AddRange(rEmailAddress.Errors);

        var rFirstName = GuestName.Create(firstName);
        if (rFirstName.IsFailure) errors.AddRange(rFirstName.Errors);

        var rLastName = GuestName.Create(lastName);
        if (rLastName.IsFailure) errors.AddRange(rLastName.Errors);

        var rPictureUrl = ProfilePictureUrl.Create(pictureUrl);
        if (rPictureUrl.IsFailure) errors.AddRange(rPictureUrl.Errors);

        if (errors.Count > 0)
            return Result<RegisterGuestCommand>.Failure(errors);

        var cmd = new RegisterGuestCommand(rGuestId.Payload!, rEmailAddress.Payload!, rFirstName.Payload!, rLastName.Payload!, rPictureUrl.Payload!);

        return Result<RegisterGuestCommand>.Success(cmd);
    }
}