using Microsoft.Extensions.DependencyInjection;
using UnitTests.Helpers;
using Xunit;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;

namespace IntegrationTests.Repositories;

public class EventRepositoryScalarUpdateTests : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _fx;
    public EventRepositoryScalarUpdateTests(ServiceProviderFixture fx) => _fx = fx;

    [Fact]
    public async Task Scalar_updates_persist_across_scopes()
    {
        // ---------- Arrange ----------
        var id = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(id)
            .WithVisibility(EventVisibility.Private)
            .Build();

        // choose new scalar values
        var newTitle       = EventTitle.Create("Updated Title").Payload!;
        var newDescription = EventDescription.Create("Updated description for persistence test.").Payload!;
        var newVisibility  = EventVisibility.Public;  // smart-enum instance
        var newMaxGuests   = MaxGuests.Create(25).Payload!;

        // ---------- Act (scope #1: save) ----------
        using (var s1 = _fx.Provider.CreateScope())
        {
            var repo = s1.ServiceProvider.GetRequiredService<IEventRepository>();
            var uow  = s1.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await repo.AddAsync(ev);
            await uow.SaveChangesAsync();
        }

        
        using (var s2 = _fx.Provider.CreateScope())
        {
            var repo = s2.ServiceProvider.GetRequiredService<IEventRepository>();
            var uow  = s2.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // track the existing entity, then persist scalar changes
            // (If your repo exposes an explicit Update, you can call it; with EF it’s not required.)
            var evInRepo = await repo.GetAsync(id);
            evInRepo!.UpdateTitle(newTitle);
            evInRepo.UpdateDescription(newDescription);
            evInRepo.UpdateVisibility(newVisibility);
            evInRepo.UpdateMaxGuests(newMaxGuests);
            await uow.SaveChangesAsync();
        }

        // ---------- Assert (scope #3: reload fresh) ----------
        using (var s3 = _fx.Provider.CreateScope())
        {
            var repo = s3.ServiceProvider.GetRequiredService<IEventRepository>();
            var re   = await repo.GetAsync(id);

            Assert.NotNull(re);

            Assert.Equal(newTitle.Value, re!.Title.Value);
            Assert.Equal(newDescription.Value, re.Description.Value);
            Assert.Equal(newVisibility.Id, re.Visibility.Id);
            Assert.Equal(newMaxGuests.Value, re.MaxGuestsNo.Value);
        }
    }
}
