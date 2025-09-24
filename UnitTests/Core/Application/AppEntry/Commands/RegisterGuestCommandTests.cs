using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class RegisterGuestCommandTests
{
    [Fact]
    public void RegisterGuest_Succeeds_When_All_Fields_Are_Valid()
    {
        // Arrange - see the class private fields
        var guestId = Guid.NewGuid().ToString();
        const string emailAddress = "308817@via.dk";
        const string firstName = "Tomasz";
        const string lastName = "Grzesiak";
        const string pictureUrl = "https://example.com/pic.jpg";

        // Act
        var result = RegisterGuestCommand.Create(guestId, emailAddress, firstName, lastName, pictureUrl);

        // Assert
        Assert.True(result.IsSuccess);
        var cmd = result.Payload!;

        Assert.Equal(emailAddress, cmd.EmailAddress.Value);
        Assert.Equal(firstName, cmd.FirstName.Value);
        Assert.Equal(lastName, cmd.LastName.Value);
        Assert.Equal(pictureUrl, cmd.ProfilePictureUrlAddress.Value);
    }

    [Fact]
    public void RegisterGuestCommand_Fails_When_All_Fields_Are_Invalid()
    {
        // Arrange - see the class private fields
        const string wrongGuestId = "wrong-guid";
        const string wrongEmail = "wrong-email";
        const string wrongFirstName = "T";
        const string wrongLastName = "Mc'Gregor";
        const string wrongPictureUrl = "not-a-url";

        // Act
        var result = RegisterGuestCommand.Create(wrongGuestId, wrongEmail, wrongFirstName, wrongLastName, wrongPictureUrl);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(8, result.Errors.Count);
        Assert.Contains(Error.UnParsableGuid, result.Errors);
        Assert.Contains(Error.EmailMustEndWithViaDomain, result.Errors);
        Assert.Contains(Error.EmailInvalidFormat, result.Errors);
        Assert.Contains(Error.EmailInvalidFormat, result.Errors);
        Assert.Contains(Error.EmailInvalidFormat, result.Errors);
        Assert.Contains(Error.InvalidNameFormat, result.Errors);
        Assert.Contains(Error.InvalidNameFormat, result.Errors);
        Assert.Contains(Error.InvalidProfilePictureUrlEmpty, result.Errors);
    }
}