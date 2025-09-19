using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class SetMaxNumberOfGuestsCommandTests
{
    [Fact]
    public void Create_Succeeds_When_Guid_And_Max_Guest_No_Are_Valid()
    {
        // Arrange
        var guidText = Guid.NewGuid().ToString();
        var maxGuestNo = 15;

        // Act
        var result = SetMaxNumberOfGuestsCommand.Create(guidText, maxGuestNo);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);

        // EventId is parsed correctly
        Assert.Equal(guidText, result.Payload!.EventId.Value.ToString());

        // MaxGuest VO created correctly
        Assert.Equal(maxGuestNo, result.Payload.MaxGuests.Value);
    }

    [Fact]
    public void Create_Fails_When_Guid_Is_Invalid()
    {
        // Arrange
        var invalidGuid = "not-a-guid";
        var maxGuestNo = 15;

        // Act
        var result = SetMaxNumberOfGuestsCommand.Create(invalidGuid, maxGuestNo);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(result.Payload);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e == Error.UnParsableGuid);
    }

    // Checking if domain errors are passed correctly
    [Fact]
    public void Create_Fails_When_MaxGuest_No_Is_Too_High()
    {
        // Arrange
        var guidText = Guid.NewGuid().ToString();
        var maxGuestNo = 55;

        // Act
        var result = SetMaxNumberOfGuestsCommand.Create(guidText, maxGuestNo);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(result.Payload);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e == Error.GuestsMaxNumberTooGreat);
    }
}