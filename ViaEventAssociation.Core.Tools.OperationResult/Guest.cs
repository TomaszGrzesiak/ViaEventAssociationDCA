namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Guest
{
    public Guid Id { get; } = Guid.NewGuid();
    private string Name { get; set; }
    private string Email { get; set; }
    private Uri ProfilePictureUrl { get; set; }

    private Guest(string email, string firstName, string lastName, Uri profilePicture)
    {
        Id = Guid.NewGuid();
        Email = email;
        Name = $"{firstName} {lastName}";
        ProfilePictureUrl = profilePicture;
    }

    public static Result CreateGuest(string email, string firstName, string lastName, Uri profilePicture)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return new Result("Invalid input for creating a guest.");
        }

        Guest guest = new Guest(email, firstName, lastName, profilePicture);
        return new Result("Guest created successfully.");
    }

    public Result ParticipateInEvent(Guid guestId, Guid eventId)
    {
        if (guestId != Id)
        {
            return new Result("Guest ID mismatch.");
        }
        return new Result("Guest successfully registered for the event.");
    }

    public Result CancelMyParticipation(Guid guestId, Guid eventId)
    {
        if (guestId != Id)
        {
            return new Result("Guest ID mismatch.");
        }
        return new Result("Guest participation canceled successfully.");
    }

    public Result AcceptInvitation(Guid guestId, Guid invitationId)
    {
        if (guestId != Id)
        {
            return new Result("Guest ID mismatch.");
        }
        return new Result("Invitation accepted successfully.");
    }

    public Result DeclineInvitation(Guid guestId, Guid invitationId)
    {
        if (guestId != Id)
        {
            return new Result("Invitation declined successfully.");
        }
        return new Result("Guest ID mismatch.");
    }
}