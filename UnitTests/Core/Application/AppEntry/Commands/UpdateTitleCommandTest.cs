using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class UpdateTitleCommandTests
{
    [Fact]
    public void Create_Succeeds_When_Guid_And_Title_Are_Valid()
    {
        // Arrange
        var guidText = Guid.NewGuid().ToString();
        var rawTitle = "  New Title  ";

        // Act
        var result = UpdateEventTitleCommand.Create(guidText, rawTitle);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);

        // EventId is parsed correctly
        Assert.Equal(guidText, result.Payload!.EventId.Value.ToString());

        // Title VO created correctly (compare via ToString to avoid internal details)
        // "New Title" is trimmed according to EventTitle.Create method in the domain.
        Assert.Equal("New Title", result.Payload.NewTitle.ToString());
    }

    [Fact]
    public void Create_Fails_When_Guid_Is_Invalid()
    {
        // Arrange
        var invalidGuid = "not-a-guid";

        // Act
        var result = UpdateEventTitleCommand.Create(invalidGuid, "Some Title");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(result.Payload);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e == Error.UnParsableGuid);
    }

    [Fact]
    public void Create_Fails_When_Title_Is_Empty()
    {
        // Arrange
        var guidText = Guid.NewGuid().ToString();

        // Act
        var result = UpdateEventTitleCommand.Create(guidText, "   ");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(result.Payload);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e == Error.EventTitleMustBeBetween3And75Characters);
    }

    [Fact]
    public void Create_Fails_With_Aggregated_Errors_When_Both_Guid_And_Title_Invalid()
    {
        // Arrange
        var invalidGuid = "nope";

        // Act
        var result = UpdateEventTitleCommand.Create(invalidGuid, "");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(result.Payload);
        Assert.True(result.Errors.Count == 2);
    }
}