using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands.Event;

public class ActivateEventCommandTests
{
    [Fact]
    public void Create_ActivateEvent_Command_Succeeds_When_Id_Is_Valid()
    {
        // Arrange
        var id = Guid.NewGuid();

        //Act
        var result = ActivateEventCommand.Create(id.ToString());

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_ActivateEvent_Command_Fails_When_Id_Is_Not_Valid()
    {
        // Arrange
        var id = "wrong id";

        //Act
        var result = ActivateEventCommand.Create(id.ToString());

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.UnParsableGuid);
    }
}