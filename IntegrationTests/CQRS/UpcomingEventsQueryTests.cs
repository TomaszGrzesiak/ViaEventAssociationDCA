using System;
using System.Linq;
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

public class UpcomingEventsQueryTests
{
    private static readonly ISystemTime FakeSystemTime =
        new FakeSystemTime(new DateTime(2024, 4, 9, 18, 0, 0));
    
    [Fact]
    public async Task UpcomingEventsQuery_Returns_PageOfUpcomingEvents()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();
        var handler = new UpcomingEventsQueryHandler(ctx, FakeSystemTime);

        var query = new UpcomingEventsQuery.Query(
            PageNumber: 1,
            PageSize: 3,
            SearchText: null);

        var result = await handler.HandleAsync(query);

        Assert.True(result.TotalEvents >= 0);
        Assert.True(result.Events.Count <= 3);

        Assert.All(result.Events, e =>
        {
            Assert.False(string.IsNullOrWhiteSpace(e.EventId));
            Assert.False(string.IsNullOrWhiteSpace(e.Title));
            Assert.True(e.MaxGuests >= 0);
            Assert.True(e.AttendeeCount >= 0);
        });
    }

    [Fact]
    public async Task UpcomingEventsQuery_Filters_By_Title()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();
        var handler = new UpcomingEventsQueryHandler(ctx, FakeSystemTime);

        var nowString = FakeSystemTime.Now().ToString("yyyy-MM-dd HH:mm");

        // getting a title of one of the stored VeaEvents
        var someTitle = await ctx.VeaEvents
            .Where(e =>
                e.TimeRangeStartTime != null &&
                e.TimeRangeStartTime!.CompareTo(nowString) >= 0)
            .Select(e => e.TitleValue)
            .FirstAsync();

        // shorting the title to a fragment
        var fragment = someTitle!.Substring(0, Math.Min(3, someTitle.Length));

        
        var query = new UpcomingEventsQuery.Query(
            PageNumber: 1,
            PageSize: 10,
            SearchText: fragment);

        var answer = await handler.HandleAsync(query);

        Assert.NotEmpty(answer.Events);
        Assert.All(answer.Events,
            e => Assert.Contains(fragment, e.Title, StringComparison.OrdinalIgnoreCase));
    }
}
