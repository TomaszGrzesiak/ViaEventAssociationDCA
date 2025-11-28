using Microsoft.Extensions.DependencyInjection;
using UnitTests.Helpers;
using Xunit;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;

namespace IntegrationTests.Repositories;

public class GuestRepositoryScalarUpdateTests : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _fx;
    public GuestRepositoryScalarUpdateTests(ServiceProviderFixture fx) => _fx = fx;

    [Fact]
    public async Task Scalar_updates_persist_across_scopes()
    {
        // ---------- Arrange ----------
        var id    = GuestId.CreateUnique();
        var guest = await GuestFactory.Init()
            .WithId(id.ToString())
            .WithEmail("alol@via.dk")
            .WithFirstName("Alice")
            .WithLastName("OldLast")
            .WithProfileUrl("https://example.com/old.png")
            .Build();

        var newEmail = EmailAddress.Create("newm@via.dk").Payload!;
        var newFirst = GuestName.Create("Aleksandra").Payload!;
        var newLast  = GuestName.Create("Newman").Payload!;
        var newPic   = ProfilePictureUrl.Create("https://example.com/new.png").Payload!;

        // ---------- Act (scope #1: save initial) ----------
        using (var s1 = _fx.Provider.CreateScope())
        {
            var repo = s1.ServiceProvider.GetRequiredService<IGuestRepository>();
            var uow  = s1.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await repo.AddAsync(guest);
            await uow.SaveChangesAsync();
        }

        // ---------- Act (scope #2: mutate + save) ----------
        using (var s2 = _fx.Provider.CreateScope())
        {
            var repo = s2.ServiceProvider.GetRequiredService<IGuestRepository>();
            var uow  = s2.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var g = await repo.GetAsync(id);
            Assert.NotNull(g);
        }

        // ---------- Assert (scope #3: reload fresh) ----------
        using (var s3 = _fx.Provider.CreateScope())
        {
            var repo = s3.ServiceProvider.GetRequiredService<IGuestRepository>();
            var re   = await repo.GetAsync(id);

            Assert.NotNull(re);
            Assert.Equal(newEmail.Value, re!.Email.Value);
            Assert.Equal(newFirst.Value, re.FirstName.Value);
            Assert.Equal(newLast.Value,  re.LastName.Value);
            Assert.Equal(newPic.Value,   re.ProfilePictureUrlAddress.Value);
        }
    }
}
