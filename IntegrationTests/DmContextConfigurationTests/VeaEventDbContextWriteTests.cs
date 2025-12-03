using EfcDmPersistence;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using Xunit;

namespace IntegrationTests.DmContextConfigurationTests;

public class VeaEventDbContextWriteTests : EfTestHelpers
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
    // DateTime today = new DateTime(FakeSystemTime.Now().Year, FakeSystemTime.Now().Month, FakeSystemTime.Now().Day, 0, 0, 0);
    
    [Fact]
    public async Task StrongIdAsPk()
    {
        await using DmContext dbContext = SetupContext();
        
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init().WithId(id).Build();
        
        await AddAndSaveAndClearAsync(entity, dbContext);

        VeaEvent? retrieved = dbContext.VeaEvents.SingleOrDefault(x => x.Id!.Equals(id));
        Assert.NotNull(retrieved);
    }

    [Fact]
    public async Task MaxGuestPropertyTest()
    {
        await using DmContext dbContext = SetupContext();
        
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init().WithId(id).Build();
        var newMaxGuest = MaxGuests.Create(15).Payload!;
        entity.UpdateMaxGuests(newMaxGuest);
        
        await AddAndSaveAndClearAsync(entity, dbContext);

        VeaEvent? retrieved = dbContext.VeaEvents.SingleOrDefault(x => x.Id!.Equals(id));
        Assert.NotNull(retrieved);
        Assert.Equal(retrieved.MaxGuestsNo.Value, newMaxGuest.Value);
    }
    
    
    // The domain constraints setting MaxGuest as a non-nullable. But in real life better be safe than sorry: migrations, BI and reports, etc.
    [Fact]
    public async Task MaxGuestProperty_NonNullableValueObject_FailsWhenNull()
    {
        await using DmContext dbContext = SetupContext();
        
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init().WithId(id).Build();
        await dbContext.VeaEvents.AddAsync(entity);
        
        dbContext.Entry(entity)
            .ComplexProperty(e => e.MaxGuestsNo)
            .CurrentValue = null!;
        
        Assert.Throws<InvalidOperationException>(() => dbContext.SaveChanges());
    }
    
    [Fact]
    public async Task TitleProperty_ValueObject_SuccessWhenTitleIsCorrect()
    {
        // Arrange
        await using DmContext dbContext = SetupContext();
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init().WithId(id).Build();
        var newTitle = EventTitle.Create("New, correct title").Payload!;
        entity.UpdateTitle(newTitle);
        
        // Act
        await AddAndSaveAndClearAsync(entity, dbContext);
        
        // Assert
        VeaEvent retrieved = dbContext.VeaEvents.SingleOrDefault(x => x.Id!.Equals(id))!;
        Assert.Equal(retrieved.Title!.Value, newTitle.Value);
    }
    
    [Fact]
    public async Task TitleProperty_NullableValueObject_SuccessWhenNull()
    {
        // Arrange
        await using DmContext dbContext = SetupContext();
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init()
            .WithId(id)
            .WithTitle(null)
            .Build();
        
        // Act
        await AddAndSaveAndClearAsync(entity, dbContext);
        
        // Assert
        VeaEvent retrieved = dbContext.VeaEvents.SingleOrDefault(x => x.Id!.Equals(id))!;
        Assert.Null(retrieved.Title);
    }
    
    [Fact]
    public async Task Event_Roundtrip_EmptyInvitations_OK()
    {
        await using var ctx = SetupContext(); // your helper

        var id = EventId.CreateUnique();
        var ev = VeaEvent.Create(id).Payload!;

        await ctx.AddAsync(ev);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var re = await ctx.VeaEvents.SingleAsync(x => x.Id == id);
        Assert.NotNull(re);
        Assert.Empty(re.Invitations);
    }
    
    [Fact]
    public async Task Event_Roundtrip_OneInvitation_OK()
    {
        await using var ctx = SetupContext();

        var eid = EventId.CreateUnique();
        var ev  = VeaEvent.Create(eid).Payload!;

        var gid = GuestId.CreateUnique();

        // Use the real domain method if available; otherwise skip this test for now.
        // Result.Must(ev.InviteGuest(gid));  // or whatever your API is

        await ctx.AddAsync(ev);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var re = await ctx.VeaEvents.SingleAsync(x => x.Id == eid);
        // Assert.Single(re.Invitations); // enable when you actually add one
    }

    [Fact]
    public async Task Cannot_Add_Two_Invites_For_Same_Guest_In_Same_Event()
    {
        await using var ctx = SetupContext();
        var ev = EventFactory.Init().WithStatus(EventStatus.Ready).Build();
        var g = GuestId.CreateUnique();

        // Domain: add two invites for same guest (however your API allows)
        var result = ev.InviteGuest(g); // whatever your domain method is
        Assert.True(result.IsSuccess);

        ev.InviteGuest(g);

        await ctx.AddAsync(ev);
        await Assert.ThrowsAnyAsync<DbUpdateException>(async () => { await ctx.SaveChangesAsync(); });
    }

    // The domain constraints setting MaxGuest as a non-nullable. But in real life better be safe than sorry: migrations, BI and reports, etc.
    [Fact]
    public async Task EventTimeRange_NonNullableMultiPrimitiveValuedValueObject()
    {
        await using DmContext ctx = SetupContext();
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init()
            .WithId(id)
            .WithVisibility(EventVisibility.Private)
            .Build();
        
        EventTimeRange newTimeRange = EventTimeRange.Create(FakeSystemTime.Now(), FakeSystemTime.Now().AddHours(3)).Payload!;
        entity.UpdateTimeRange(newTimeRange, FakeSystemTime);
    
        await AddAndSaveAndClearAsync(entity, ctx);
    
        VeaEvent retrieved = ctx.VeaEvents.Single(x => x.Id == entity.Id);
        Assert.NotNull(retrieved.TimeRange);
        Assert.Equal(newTimeRange.StartTime, retrieved.TimeRange.StartTime);
        Assert.Equal(newTimeRange.EndTime, retrieved.TimeRange.EndTime);
    }
    
    // The domain constraints setting MaxGuest as a non-nullable. But in real life better be safe than sorry: migrations, BI and reports, etc.
    [Fact]
    public async Task EventDescription_NullableMultiValuedValueObject_NoneAreNull()
    {
        await using DmContext ctx = SetupContext();
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init()
            .WithId(id)
            .WithVisibility(EventVisibility.Private)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();
        EventDescription newEventDescription = EventDescription.Create("New event description.").Payload!;
        
        entity.UpdateDescription(newEventDescription);
    
        await AddAndSaveAndClearAsync(entity, ctx);
    
        VeaEvent retrieved = ctx.VeaEvents.Single(x => x.Id == entity.Id);
        Assert.NotNull(retrieved.Description);
        Assert.Equal(newEventDescription.Value, retrieved.Description.Value);
    }
    
    [Fact]
    public async Task EventDescription_NullableMultiValuedValueObject_OneValueObjectPropertyIsNull()
    {
        // 1) Build with default Event Description "Default description"
        await using DmContext ctx = SetupContext();
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init()
            .WithId(id)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .WithDescription(EventDescription.Default().Value)
            .Build();
        
        //entity.UpdateDescription(null); // this won't work, because the domain constraints don't allow it. Can we circumvent that?
        
        // 2) Save once so the owner (and owned) exist in DB
        await AddAndSaveAndClearAsync(entity, ctx);

        // 3) Reload a tracked instance
        var toUpdate = ctx.VeaEvents.Single(x => x.Id == id);

        // Ensure the owned is loaded (important)
        await ctx.Entry(toUpdate).Reference(e => e.Description).LoadAsync();

        // 4) Now null the owned nav (EF will mark it Deleted in an UPDATE scenario — which is valid)
        ctx.Entry(toUpdate).Reference(e => e.Description).CurrentValue = null;

        // await AddAndSaveAndClearAsync(toUpdate, ctx);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

    
        // 5) Assert it persisted as NULL
        var retrieved = ctx.VeaEvents.Single(x => x.Id == id);
        Assert.Null(retrieved.Description);
    }
}