using ViaEventAssociation.Core.Tools.OperationResult;

namespace Domain.Aggregates.Guest;

public class Guest
{
    // TODO: make Value objects first according to the domain model and assign them to the properties below:
    private Guid Id { get; } = Guid.NewGuid();
    private string Name { get; set; }
    private string Email { get; set; }
    private Uri ProfilePictureUrl { get; set; }

    private Guest(string email, string firstName, string lastName, Uri profilePicture)
    {
        // TODO: add proper validation based on the use cases / requirements
        Email = email;
        Name = $"{firstName} {lastName}";
        ProfilePictureUrl = profilePicture;
    }

    public static Result<Guest> CreateGuest(string email, string firstName, string lastName, Uri profilePicture)
    {
        // TODO: check if all validation is implemented
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return Result<Guest>.Failure("Invalid input for creating a guest.");
        }

        Guest guest = new Guest(email, firstName, lastName, profilePicture);
        return Result<Guest>.Success(guest);
    }

    public Result<Guest> ParticipateInEvent(Guid guestId, Guid eventId)
    {
        // TODO: add proper implementation of participation
        if (guestId != Id)
        {
            return Result<Guest>.Failure("Guest ID mismatch.");
        }
        return Result<Guest>.Success(this);
    }

    public Result<Guest> CancelMyParticipation(Guid guestId, Guid eventId)
    {
        // TODO: add proper implementation of canceling the participation
        if (guestId != Id)
        {
            return Result<Guest>.Failure("Guest ID mismatch.");
        }
        return Result<Guest>.Success(this);
    }

    public Result<Guest> AcceptInvitation(Guid guestId, Guid invitationId)
    {
        // TODO: add proper implementation of accepting the invitation
        if (guestId != Id)
        {
            return Result<Guest>.Failure("Guest ID mismatch.");
        }
        return Result<Guest>.Success(this);
    }

    public Result<Guest> DeclineInvitation(Guid guestId, Guid invitationId)
    {
        // TODO: add proper implementation of declining the invitation
        if (guestId != Id)
        {
            return Result<Guest>.Failure("Guest ID mismatch.");
        }
        return Result<Guest>.Success(this);
    }
}