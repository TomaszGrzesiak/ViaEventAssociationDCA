using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class CreateEventCommandTest
{
    [Fact]
    public void Create_Succeeds_And_Parses_Id()
    {
        // Arrange
        var guidText = Guid.NewGuid().ToString();

        // Act
        var result = CreateEventCommand.Create(guidText);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);

        var cmd = result.Payload as CreateEventCommand;
        Assert.Equal(cmd._eventId.ToString(), guidText);
    }
}