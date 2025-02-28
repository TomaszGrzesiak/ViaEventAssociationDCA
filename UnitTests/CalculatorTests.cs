using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests;

public class CalculatorTests
{
    [Fact]
    public void Sum_ReturnsCorrectSum()
    {
        // Arrange
        var calculator = new Calculator(5, 7);

        // Act
        var result = calculator.Sum();

        // Assert
        Assert.Equal(12, result);
    }
}