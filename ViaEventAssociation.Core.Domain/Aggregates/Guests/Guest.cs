using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public sealed class Guest : AggregateRoot<GuestId>
{
    public GuestName Name { get; private set; }
    public EmailAddress Email { get; private set; }
    public ProfilePictureUrl ProfilePicture { get; private set; }

    private Guest(GuestId id, GuestName name, EmailAddress email, ProfilePictureUrl profilePicture)
        : base(id)
    {
        Name = name;
        Email = email;
        ProfilePicture = profilePicture;
    }

    public static Result<Guest> Create(
        GuestName name,
        EmailAddress email,
        ProfilePictureUrl profilePicture)
    {
        var newGuest = new Guest(GuestId.CreateUnique(), name, email, profilePicture);
        return Result<Guest>.Success(newGuest);
    }

    public override string ToString() =>
        $"{Name.ToString()} ({Email.ToString()})";
}