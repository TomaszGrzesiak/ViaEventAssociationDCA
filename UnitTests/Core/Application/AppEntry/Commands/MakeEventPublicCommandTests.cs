using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class MakeEventPublicCommandTests
{
    [Fact]
    public void Create_Succeeds_When_Guid_Is_Valid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result = MakeEventPublicCommand.Create(guid.ToString());

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_Fails_When_Guid_Is_InValid()
    {
        // Arrange
        var guid = "invalid";

        // Act
        var result = MakeEventPublicCommand.Create(guid);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.UnParsableGuid);
    }
}