using System;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using Xunit;
using Xunit.Abstractions;

public class GuestTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GuestTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Create_WithValidValueObjects_ReturnsSuccessGuest()
    {
        // Arrange
        var nameResult = GuestName.Create("alice", "jensen");
        var emailResult = EmailAddress.Create("abc@via.dk");
        var pictureResult = ProfilePictureUrl.Create("https://example.com/image.jpg");

        Assert.True(nameResult.IsSuccess);
        Assert.True(emailResult.IsSuccess);
        Assert.True(pictureResult.IsSuccess);

        // Act
        var guestResult = Guest.Create(nameResult.Payload!, emailResult.Payload!, pictureResult.Payload!);

        // Assert
        Assert.True(guestResult.IsSuccess);
        var guest = guestResult.Payload!;
        Assert.NotNull(guest);
        Assert.NotEqual(default, guest.Id);
        Assert.Equal("Alice", guest.Name.FirstName);
        Assert.Equal("Jensen", guest.Name.LastName);
        Assert.Equal("abc@via.dk", guest.Email.Value);
        Assert.Equal("https://example.com/image.jpg", guest.ProfilePicture.Value.ToString());
    }

    [Fact]
    public void ToString_ReturnsFormattedFullNameAndEmail()
    {
        // Arrange
        var name = GuestName.Create("bOB", "andERsen").Payload!;
        var email = EmailAddress.Create("cde@via.dk").Payload!;
        var picture = ProfilePictureUrl.Create("https://example.com/pic.png").Payload!;

        var guest = Guest.Create(name, email, picture).Payload!;

        _testOutputHelper.WriteLine(guest.ToString());
        // Act
        var result = guest.ToString();

        // Assert
        Assert.Equal("Bob Andersen (cde@via.dk)", result);
    }
}