using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Application.Features.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace UnitTests.Core.Application.Features.Guest;

public class RegisterGuestHandlerTests
{
    [Fact]
    public async Task HandleAsync_RegistersGuest_AndSaves()
    {
        // Arrange
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var checker = new FakeUniqueEmailChecker([]);
        var handler = new RegisterGuestHandler(guestRepo, uow, checker);

        var guestId = GuestId.CreateUnique();
        const string emailAddress = "308817@via.dk";
        const string firstName = "Tomasz";
        const string lastName = "Grzesiak";
        const string pictureUrl = "https://example.com/pic.jpg";

        var cmd = RegisterGuestCommand.Create(guestId.ToString(), emailAddress, firstName, lastName, pictureUrl).Payload!;

        // Act
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsSuccess);

        var guest = await guestRepo.GetAsync(guestId);

        Assert.NotNull(guest);
        Assert.Equal(guest.Id, guestId);

        Assert.Equal(pictureUrl, guest.ProfilePictureUrlAddress.ToString());
        Assert.Equal(firstName, guest.FirstName.ToString());
        Assert.Equal(lastName, guest.LastName.ToString());
        Assert.Equal(emailAddress, guest.Email.ToString());
    }
}