using Microsoft.Extensions.DependencyInjection;
using UnitTests.Helpers;
using Xunit;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;

namespace IntegrationTests.Repositories;

public class GuestScalarValuesPersistAcrossScopes : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _fx;
    public GuestScalarValuesPersistAcrossScopes(ServiceProviderFixture fx) => _fx = fx;

    [Fact]
    public async Task Guest_scalar_values_persist_across_scopes_and_remain_unchanged()
    {
        var id    = GuestId.CreateUnique();
        var email = EmailAddress.Create("alol@via.dk").Payload!;
        var first = GuestName.Create("Alice").Payload!;
        var last  = GuestName.Create("OldLast").Payload!;
        var pic   = ProfilePictureUrl.Create("https://example.com/old.png").Payload!;

        var guest = await GuestFactory.Init()
            .WithId(id.ToString())
            .WithEmail(email.Value)
            .WithFirstName(first.Value)
            .WithLastName(last.Value)
            .WithProfileUrl(pic.Value)
            .Build();

        // scope 1: save
        using (var s1 = _fx.Provider.CreateScope())
        {
            var repo = s1.ServiceProvider.GetRequiredService<IGuestRepository>();
            var uow  = s1.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await repo.AddAsync(guest);
            await uow.SaveChangesAsync();
        }

        // scope 2: reload and verify values are the same
        using (var s2 = _fx.Provider.CreateScope())
        {
            var repo = s2.ServiceProvider.GetRequiredService<IGuestRepository>();
            var g    = await repo.GetAsync(id);

            Assert.NotNull(g);
            Assert.Equal(email.Value, g!.Email.Value);
            Assert.Equal(first.Value, g.FirstName.Value);
            Assert.Equal(last.Value,  g.LastName.Value);
            Assert.Equal(pic.Value,   g.ProfilePictureUrlAddress.Value);
        }
    }

}
