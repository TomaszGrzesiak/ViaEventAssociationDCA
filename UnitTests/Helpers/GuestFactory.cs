using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Helpers;

public class GuestFactory
{
    private static int _testMailCounter = 100000; // to avoid collisions with existing guests, when tests create multiple example guests
    private EmailAddress _email = EmailAddress.Create($"{_testMailCounter++}@via.dk").Payload!;
    private GuestName _firstName = GuestName.Create("Geralt").Payload!;
    private GuestName _lastName = GuestName.Create("Gwynbleidd").Payload!;
    private ProfilePictureUrl _picture = ProfilePictureUrl.Create("https://static.wikia.nocookie.net/witcher/images/9/9b/Geralt-cool.jpg").Payload!;
    private GuestId _guestId = GuestId.CreateUnique();
    private IEmailUnusedChecker _checker = new FakeUniqueEmailChecker([]);


    public static GuestFactory Init() => new();

    public GuestFactory WithId(string guid)
    {
        _guestId = GuestId.FromString(guid).Payload!;
        return this;
    }

    public GuestFactory WithEmail(string email)
    {
        _email = EmailAddress.Create(email).Payload!;
        return this;
    }

    public GuestFactory WithFirstName(string name)
    {
        _firstName = GuestName.Create(name).Payload!;
        return this;
    }

    public GuestFactory WithLastName(string name)
    {
        _lastName = GuestName.Create(name).Payload!;
        return this;
    }

    public GuestFactory WithProfileUrl(string url)
    {
        _picture = ProfilePictureUrl.Create(url).Payload!;
        return this;
    }

    public GuestFactory WithChecker(IEmailUnusedChecker checker)
    {
        _checker = checker;
        return this;
    }


    public async Task<Guest> Build()
    {
        var guest = await Guest.Register(_guestId, _email, _firstName, _lastName, _picture, _checker);
        return guest.Payload!;
    }
}