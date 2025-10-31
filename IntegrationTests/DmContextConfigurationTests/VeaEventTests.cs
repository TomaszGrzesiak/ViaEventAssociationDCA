using EfcDmPersistence;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using Xunit;

namespace IntegrationTests.DmContextConfigurationTests;

public class VeaEventTests
{
    [Fact]
    public async Task StrongIdAsPk()
{
    await using DmContext ctx = GlobalUsings.SetupContext();
    
    EventId id = EventId.CreateUnique();
    VeaEvent entity = EventFactory.Init().WithId(id).Build();
    
    await EfTestHelpers.SaveAndClearAsync(entity, ctx);

    VeaEvent? retrieved = ctx.Events.SingleOrDefault(x => x.Id!.Equals(id));
    Assert.NotNull(retrieved);
}
}