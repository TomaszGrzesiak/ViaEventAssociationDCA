using System;
using System.Threading.Tasks;
using EfcQueries;
using EfcQueries.Models;
using EfcQueries.Queries;
using EfcQueries.SeedFactories;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;
using Xunit;

namespace IntegrationTests.CQRS;

public class GuestProfileQueryTests
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
    
    [Fact]
    public async Task GuestProfileQuery_ReturnsUpcomingAndPastEvents()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();

        // pick any guest that actually has participations
        var someGuestId = await ctx.EventParticipants
            .Select(ep => ep.GuestId)
            .FirstAsync();

        
        var handler = new GuestProfileQueryHandler(ctx, FakeSystemTime);

        var result = await handler.HandleAsync(new GuestProfileQuery.Query(someGuestId));

        Assert.Equal(someGuestId, result.GuestId);
        Assert.True(result.UpcomingEventsCount >= 0);
        Assert.True(result.UpcomingEvents.Count == result.UpcomingEventsCount);
        Assert.True(result.PastEvents.Count <= 5);
    }
}