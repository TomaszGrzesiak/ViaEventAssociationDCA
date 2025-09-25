using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class InviteGuestCommandTests
{
    [Fact]
    public void Create_Succeeds_And_Parses_Id()
    {
        // Arrange
        var eventGuid = Guid.NewGuid().ToString();
        var guestGuid = Guid.NewGuid().ToString();

        // Act
        var result = InviteGuestCommand.Create(eventGuid, guestGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);

        var cmd = result.Payload!;
        Assert.Equal(cmd.EventId.ToString(), eventGuid);
        Assert.Equal(cmd.GuestId.ToString(), guestGuid);
    }

    [Fact]
    public void Create_Fails_To_Parse_Id()
    {
        // Arrange
        var eventGuid = "not-a-guid";
        var guestGuid = "not-a-guid";

        // Act
        var result = InviteGuestCommand.Create(eventGuid, guestGuid);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(Error.UnParsableGuid, result.Errors);
    }
}