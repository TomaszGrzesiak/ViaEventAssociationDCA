using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using Xunit;

namespace IntegrationTests.DmContextConfigurationTests;

public class GuestDbContextWriteTests : EfTestHelpers

{
    [Fact]
    public async Task Guest_Roundtrip_OK()
    {
        await using var ctx = SetupContext();

        var id = GuestId.CreateUnique();
        var email = EmailAddress.Create("ALLI@via.dk").Payload!;
        var firstName = GuestName.Create("Alice").Payload!;
        var lastName = GuestName.Create("Liddell").Payload!;
        var profilePictureUrl = ProfilePictureUrl.Create("https://example.com/p.png").Payload!;
        var guest = await GuestFactory.Init()
            .WithId(id.ToString())
            .WithEmail(email.ToString())
            .WithFirstName(firstName.ToString())
            .WithLastName(lastName.ToString())
            .WithProfileUrl(profilePictureUrl.ToString())
            .Build();

        await ctx.AddAsync(guest);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var loaded = await ctx.Guests.SingleAsync(g => g.Id == id);
        Assert.Equal(email, loaded.Email);
        Assert.Equal(firstName, loaded.FirstName);
        Assert.Equal(lastName, loaded.LastName);
        Assert.Equal(profilePictureUrl, loaded.ProfilePictureUrlAddress);
    }
}