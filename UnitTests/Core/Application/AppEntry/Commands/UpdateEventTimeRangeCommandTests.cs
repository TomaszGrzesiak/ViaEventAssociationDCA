using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.AppEntry.Commands;

public class UpdateEventTimeRangeCommandTests
{
    // private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
    // DateTime today = new DateTime(FakeSystemTime.Now().Year, FakeSystemTime.Now().Month, FakeSystemTime.Now().Day, 0, 0, 0);

    [Fact]
    public void Update_Succeeds_WithValidInputs()
    {
        //Arrange
        var guid = Guid.NewGuid();
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();

        var startTime = "2023-08-25 19:00";
        var endTime = "2023-08-25 23:59";

        //Act
        var result = UpdateEventTimeRangeCommand.Create(guid.ToString(), startTime, endTime);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(guid.ToString(), result.Payload!.EventId.ToString());
        Assert.Equal(DateTime.Parse(startTime), result.Payload!.EventTimeRange.StartTime);
        Assert.Equal(DateTime.Parse(endTime), result.Payload!.EventTimeRange.EndTime);
    }

    [Fact]
    public void Update_Fails_WithInvalidStartOrEndTimeFormat()
    {
        // Arrange
        var guid = Guid.NewGuid();

        var badStart = "not-a-date";
        var badEnd = "also-not-a-date";

        // Act
        var result1 = UpdateEventTimeRangeCommand.Create(guid.ToString(), badStart, "2023-08-25 23:59");
        var result2 = UpdateEventTimeRangeCommand.Create(guid.ToString(), "2023-08-25 19:00", badEnd);

        // Assert
        Assert.True(result1.IsFailure);
        Assert.Contains(Error.TimeNotParsable, result1.Errors);

        Assert.True(result2.IsFailure);
        Assert.Contains(Error.TimeNotParsable, result2.Errors);
    }
}