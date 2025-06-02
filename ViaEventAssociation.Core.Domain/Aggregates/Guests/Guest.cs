using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public sealed class Guest : AggregateRoot<GuestId>
{
    public EmailAddress Email { get; private set; }
    public GuestName FirstName { get; private set; }
    public GuestName LastName { get; private set; }
    public ProfilePictureUrl ProfilePictureUrlAddress { get; private set; }

    private static readonly List<Guest> _guests = []; // temporary storage for the guests

    private Guest(GuestId id, EmailAddress email, GuestName firstName, GuestName lastName, ProfilePictureUrl profilePictureUrlAddress)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrlAddress = profilePictureUrlAddress;
    }

    public static Result<Guest> Register(
        EmailAddress email,
        GuestName firstName,
        GuestName lastName,
        ProfilePictureUrl profilePicture)
    {
        var errors = new List<Error>();

        if (EmailAddress.Validate(email) is { IsFailure: true } result1) errors.AddRange(result1.Errors);
        if (GuestName.Validate(firstName) is { IsFailure: true } result2) errors.AddRange(result2.Errors);
        if (GuestName.Validate(lastName) is { IsFailure: true } result3) errors.AddRange(result3.Errors);
        if (ProfilePictureUrl.Validate(profilePicture) is { IsFailure: true } result4) errors.AddRange(result4.Errors);

        if (errors.Count > 0)
            return Result<Guest>.Failure(errors.ToArray());

        // if no errors:
        var newGuest = new Guest(GuestId.CreateUnique(), email, firstName, lastName, profilePicture);
        if (_guests.Any(g => g.Email.Value == email.Value)) return Result<Guest>.Failure(Error.EmailAlreadyRegistered);

        _guests.Add(newGuest);
        return Result<Guest>.Success(newGuest);
    }

    public override string ToString() =>
        $"{FirstName.ToString()} {LastName.ToString()} ({Email.ToString()})";
}