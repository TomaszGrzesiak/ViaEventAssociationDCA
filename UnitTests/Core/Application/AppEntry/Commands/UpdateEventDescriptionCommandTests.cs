using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public sealed class UpdateEventDescriptionCommandTests
{
    [Fact]
    public void Update_Succeeds_WithValidInputs()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();
        var desc = "Snacks provided. Bring your own alcohol.";

        // Act
        var result = UpdateEventDescriptionCommand.Create(guid, desc);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.IsType<UpdateEventDescriptionCommand>(result.Payload!);

        var parsedId = EventId.FromString(guid).Payload!;
        Assert.Equal(parsedId, result.Payload!.EventId);
        Assert.Equal(desc, result.Payload!.EventDescription.Value);
    }

    [Fact]
    public void Create_Succeeds_WhenDescriptionIsNull_TreatedAsEmpty()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();
        string? desc = null;

        // Act
        var result = UpdateEventDescriptionCommand.Create(guid, desc!);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Payload!.EventDescription.Value); // null -> "" by VO
    }

    [Fact]
    public void Create_Fails_WhenGuidIsInvalid()
    {
        // Arrange
        var badGuid = "not-a-guid";
        var desc = "Something";

        // Act
        var result = UpdateEventDescriptionCommand.Create(badGuid, desc);

        // Assert
        Assert.True(result.IsFailure);
        // We don’t assert a specific error constant since it comes from EventId.FromString
        Assert.NotEmpty(result.Errors); // at least one error reported
    }

    [Fact]
    public void Create_Fails_WhenDescriptionTooLong()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();
        var tooLong = new string('a', 251); // 251 > 250

        // Act
        var result = UpdateEventDescriptionCommand.Create(guid, tooLong);

        // Assert
        Assert.True(result.IsFailure);
        // This comes from EventDescription.Create validation (max 250 chars)
        // Error constant: Error.EventDescriptionCannotExceed250Characters
        Assert.Contains(result.Errors, e => e == Error.EventDescriptionCannotExceed250Characters);
    }
}