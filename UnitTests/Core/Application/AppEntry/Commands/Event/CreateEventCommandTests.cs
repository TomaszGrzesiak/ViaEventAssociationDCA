using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

namespace UnitTests.Core.Application.AppEntry.Commands.Event;

public class CreateEventCommandTests
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
        Assert.Equal(cmd.EventId.ToString(), guidText);
    }
}