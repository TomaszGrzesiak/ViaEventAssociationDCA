using IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using Xunit;

namespace IntegrationTests.Repositories;

public class EventRepositorySmokeTests(ServiceProviderFixture fx) : EfTestHelpers, IClassFixture<ServiceProviderFixture>
{
    //private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
    //DateTime today = new DateTime(FakeSystemTime.Now().Year, FakeSystemTime.Now().Month, FakeSystemTime.Now().Day, 0, 0, 0);
    
[Fact]
public async Task Save_Then_Reload_Event_EmptyCollections()
{
    // scope 1 — write
    using var s1 = fx.Provider.CreateScope();
    var eRepo = s1.ServiceProvider.GetRequiredService<IEventRepository>();
    var uow   = s1.ServiceProvider.GetRequiredService<IUnitOfWork>();

    var id = EventId.CreateUnique();
    var ev = VeaEvent.Create(id).Payload!;

    await eRepo.AddAsync(ev);
    await uow.SaveChangesAsync();

    // scope 2 — read (fresh DbContext, no tracking lies)
    using var s2 = fx.Provider.CreateScope();
    var eRepo2 = s2.ServiceProvider.GetRequiredService<IEventRepository>();
    var loaded = await eRepo2.GetAsync(id);

    Assert.NotNull(loaded);
    Assert.Empty(loaded!.Invitations);
    Assert.Empty(loaded.GuestList);
}

}