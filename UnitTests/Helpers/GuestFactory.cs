using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace UnitTests.Helpers;

public class GuestFactory
{
    private static int _testMailCounter = 100000; // to avoid collisions with existing guests, when tests create multiple example guests
    private EmailAddress _email = EmailAddress.Create($"{_testMailCounter++}@via.dk").Payload!;
    private GuestName _firstName = GuestName.Create("Geralt").Payload!;
    private GuestName _lastName = GuestName.Create("Gwynbleidd").Payload!;
    private ProfilePictureUrl _picture = ProfilePictureUrl.Create("https://static.wikia.nocookie.net/witcher/images/9/9b/Geralt-cool.jpg").Payload!;


    public static GuestFactory Init() => new();

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

    public async Task<Guest> Build()
    {
        var checker = new FakeUniqueEmailChecker([]);
        var guest = await Guest.Register(_email, _firstName, _lastName, _picture, checker);
        return guest.Payload!;
    }
}