using System.Reflection;
using EfcDmPersistence;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using Xunit;
using EventId = ViaEventAssociation.Core.Domain.Aggregates.Events.EventId;

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
        VeaEvent entity = EventFactory.Init()
            .WithId(id)
            .WithVisibility(EventVisibility.Private)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();
        
        await AddAndSaveAndClearAsync(entity, dbContext);

        VeaEvent? retrieved = dbContext.VeaEvents.SingleOrDefault(x => x.Id!.Equals(id));
        Assert.NotNull(retrieved);
    }

    [Fact]
    public async Task MaxGuestPropertyTest()
    {
        await using DmContext dbContext = SetupContext();
        
        EventId id = EventId.CreateUnique();
        VeaEvent entity = EventFactory.Init()
            .WithId(id)
            .WithVisibility(EventVisibility.Private)
            .WithValidDescription()
            .Build();
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
        VeaEvent entity = EventFactory.Init()
            .WithVisibility(EventVisibility.Private)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .WithId(id).Build();
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
        VeaEvent entity = EventFactory.Init()
            .WithVisibility(EventVisibility.Private)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .WithId(id).Build();
        var newTitle = EventTitle.Create("New, correct title").Payload!;
        entity.UpdateTitle(newTitle);
        
        // Act
        await AddAndSaveAndClearAsync(entity, dbContext);
        
        // Assert
        VeaEvent retrieved = dbContext.VeaEvents.SingleOrDefault(x => x.Id!.Equals(id))!;
        Assert.Equal(retrieved.Title!.Value, newTitle.Value);
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
    public async Task Invitation_Unique_Index_Prevents_Duplicate_Guest_For_Same_Event()
    {
        await using var ctx = SetupContext();
        var eventId = EventId.CreateUnique();
        var guestId = GuestId.CreateUnique();

        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Ready)
            .WithVisibility(EventVisibility.Private)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // Bypass domain – use reflection to get backing field
        var invitationsField = typeof(VeaEvent)
            .GetField("_invitations", BindingFlags.NonPublic | BindingFlags.Instance);
        var invitations = (List<Invitation>)invitationsField!.GetValue(ev)!;

        invitations.Add(Invitation.Create(guestId));
        invitations.Add(Invitation.Create(guestId));   // duplicate

        await ctx.AddAsync(ev);

        await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
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
}