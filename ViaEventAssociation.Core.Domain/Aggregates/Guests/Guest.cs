using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public class Guest : AggregateRoot<GuestId>
{
    public EmailAddress Email { get; private set; }
    public GuestName FirstName { get; private set; }
    public GuestName LastName { get; private set; }
    public ProfilePictureUrl ProfilePictureUrlAddress { get; private set; }

    private Guest() {} // EF-only
    private Guest(GuestId id, EmailAddress email, GuestName firstName, GuestName lastName, ProfilePictureUrl profilePictureUrlAddress)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrlAddress = profilePictureUrlAddress;
    }

    public static async Task<Result<Guest>> Register(
        GuestId guestId,
        EmailAddress emailAddress,
        GuestName firstName,
        GuestName lastName,
        ProfilePictureUrl profilePicture,
        IEmailUnusedChecker checker
    )
    {
        // F5 – uniqueness (domain rule via port)
        var isUnique = await checker.IsUniqueAsync(emailAddress);
        if (!isUnique) return Result<Guest>.Failure(Error.EmailAlreadyRegistered);

        var newGuest = new Guest(guestId, emailAddress, firstName, lastName, profilePicture);

        return Result<Guest>.Success(newGuest);
    }

    public override string ToString() =>
        $"{FirstName.ToString()} {LastName.ToString()} ({Email.ToString()})";
}