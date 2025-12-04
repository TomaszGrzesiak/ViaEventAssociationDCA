using System.Threading.Tasks;
using EfcQueries;
using EfcQueries.Models;
using EfcQueries.Queries;
using EfcQueries.SeedFactories;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Queries;
using Xunit;

namespace IntegrationTests.CQRS;

public class EventDetailsQueryTests
{
    [Fact]
    public async Task EventDetailsQuery_Returns_Event_Info_And_Guest_Window_No_Invitations()
    {
        await using var ctx = ReadModelTestHelpers
            .SetupReadContext()
            .Seed();

        // Pick an event that has participants
        
        var someEventId = "40ed2fd9-2240-4791-895f-b9da1a1f64e4";
        var totalParticipants = 27;

        var handler = new EventDetailsQueryHandler(ctx);

        var query = new EventDetailsQuery.Query(
            EventId: someEventId,
            Offset: 0,
            PageSize: 9);

        var answer = await handler.HandleAsync(query);

        Assert.Equal(someEventId, answer.EventId);
        Assert.False(string.IsNullOrWhiteSpace(answer.Title));
        Assert.False(string.IsNullOrWhiteSpace(answer.Description));
        Assert.True(answer.AttendeeCount == totalParticipants);
        Assert.True(answer.MaxGuests >= answer.AttendeeCount);

        Assert.True(answer.TotalGuests >= answer.Guests.Count);
        Assert.InRange(answer.Offset, 0, answer.TotalGuests);
    }
    
    [Fact]
    public async Task EventDetailsQuery_Returns_Event_Info_And_Guest_Window_With_Invitation()
    {
        await using var ctx = ReadModelTestHelpers
            .SetupReadContext()
            .Seed();

        // Pick an event that has participants
        
        var someEventId = "27a5bde5-3900-4c45-9358-3d186ad6b2d7";
        var acceptedInvitation = 8;

        var handler = new EventDetailsQueryHandler(ctx);

        var query = new EventDetailsQuery.Query(
            EventId: someEventId,
            Offset: 0,
            PageSize: 9);

        var answer = await handler.HandleAsync(query);

        Assert.Equal(someEventId, answer.EventId);
        Assert.False(string.IsNullOrWhiteSpace(answer.Title));
        Assert.False(string.IsNullOrWhiteSpace(answer.Description));
        Assert.True(answer.AttendeeCount == acceptedInvitation);
        Assert.True(answer.MaxGuests >= answer.AttendeeCount);

        Assert.True(answer.TotalGuests >= answer.Guests.Count);
        Assert.InRange(answer.Offset, 0, answer.TotalGuests);
    }
}