using System.Linq;
using System.Threading.Tasks;
using EfcQueries;
using EfcQueries.Models;
using EfcQueries.SeedFactories;
using EfcQueries.Queries;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Queries;
using Xunit;

namespace IntegrationTests.CQRS;

public class UnpublishedEventsOverviewQueryTests
{
    private const int StatusDraft     = 1;
    private const int StatusCancelled = 3;
    private const int StatusReady     = 4;

    [Fact]
    public async Task UnpublishedEventsOverviewQuery_Splits_Events_By_Status()
    {
        await using var ctx = ReadModelTestHelpers
            .SetupReadContext()
            .Seed();

        var handler = new UnpublishedEventsOverviewQueryHandler(ctx);

        var answer = await handler.HandleAsync(new UnpublishedEventsOverviewQuery.Query());

        // Basic sanity
        Assert.NotNull(answer);
        Assert.NotNull(answer.Drafts);
        Assert.NotNull(answer.Ready);
        Assert.NotNull(answer.Cancelled);

        // Compare with direct DB queries so we know the LINQ is right.
        var expectedDrafts = await ctx.VeaEvents
            .Where(e => e.Status == StatusDraft)
            .CountAsync();
        var expectedReady = await ctx.VeaEvents
            .Where(e => e.Status == StatusReady)
            .CountAsync();
        var expectedCancelled = await ctx.VeaEvents
            .Where(e => e.Status == StatusCancelled)
            .CountAsync();

        Assert.Equal(expectedDrafts, answer.Drafts.Count);
        Assert.Equal(expectedReady, answer.Ready.Count);
        Assert.Equal(expectedCancelled, answer.Cancelled.Count);

        // Ensure no cross-contamination: IDs are unique across all three lists.
        var allIds = answer.Drafts
            .Concat(answer.Ready)
            .Concat(answer.Cancelled)
            .Select(e => e.EventId)
            .ToList();

        Assert.Equal(allIds.Count, allIds.Distinct().Count());
    }
}
