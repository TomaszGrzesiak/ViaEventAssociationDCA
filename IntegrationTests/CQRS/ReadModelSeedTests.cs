using EfcQueries;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests.CQRS;


// This test does: "Just verify in a test that you can instantiate a new database, and seed it with test data."
public class ReadModelSeedTests
{
    [Fact]
    public async Task DbHasGuestsSeed()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();
 
        Assert.NotEmpty(ctx.Guests);
        Assert.Equal(50, ctx.Guests.Count());
    }
    
    [Fact]
    public async Task DbHasEventsSeed()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();

        Assert.NotEmpty(ctx.VeaEvents);
        Assert.True(ctx.VeaEvents.Count() > 10);
    }
    
    [Fact]
    public async Task DbHasParticipationsSeed()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();

        Assert.NotEmpty(ctx.EventParticipants);

        // Every participation should reference an existing Guest and Event
        var invalidGuestRefs = ctx.EventParticipants
            .Where(ep => !ctx.Guests.Any(g => g.Id == ep.GuestId))
            .ToList();

        var invalidEventRefs = ctx.EventParticipants
            .Where(ep => !ctx.VeaEvents.Any(e => e.Id == ep.EventId))
            .ToList();

        Assert.Empty(invalidGuestRefs);
        Assert.Empty(invalidEventRefs);
    }
    
    [Fact]
    public async Task DbHasInvitationsSeed()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();

        Assert.NotEmpty(ctx.Invitations);

        // All invitations must point to existing guest + event
        var invalidGuestRefs = ctx.Invitations
            .Where(i => !ctx.Guests.Any(g => g.Id == i.GuestId))
            .ToList();

        var invalidEventRefs = ctx.Invitations
            .Where(i => !ctx.VeaEvents.Any(e => e.Id == i.EventId))
            .ToList();

        Assert.Empty(invalidGuestRefs);
        Assert.Empty(invalidEventRefs);
    }

    [Fact]
    public async Task EventStatusesAndVisibilitiesAreWithinEnumRange()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();

        var invalidStatuses = ctx.VeaEvents
            .Where(e => e.Status < 1 || e.Status > 4)
            .ToList();

        var invalidVisibilities = ctx.VeaEvents
            .Where(e => e.Visibility < 1 || e.Visibility > 2)
            .ToList();

        Assert.Empty(invalidStatuses);
        Assert.Empty(invalidVisibilities);
    }

    [Fact]
    public async Task InvitationStatusesAreWithinEnumRange()
    {
        await using var ctx = ReadModelTestHelpers.SetupReadContext().Seed();

        var invalid = ctx.Invitations
            .Where(i => i.Status < 1 || i.Status > 3)
            .ToList();

        Assert.Empty(invalid);
    }
}

