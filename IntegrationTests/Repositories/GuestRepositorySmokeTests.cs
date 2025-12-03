using EfcDmPersistence;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;
using Xunit;

namespace IntegrationTests.Repositories;


[Collection("DI")]
public class GuestRepositorySmokeTests: EfTestHelpers
{
    private readonly ServiceProviderFixture _fx;
    public GuestRepositorySmokeTests(ServiceProviderFixture fx) => _fx = fx;
    
    //private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
    //DateTime today = new DateTime(FakeSystemTime.Now().Year, FakeSystemTime.Now().Month, FakeSystemTime.Now().Day, 0, 0, 0);
    
    [Fact]
    public async Task Save_Then_Reload_Guest()
    {
        using var scope1 = _fx.Provider.CreateScope();
        var sp1 = scope1.ServiceProvider;
    
        var gRepo = sp1.GetRequiredService<IGuestRepository>();
        var uow   = sp1.GetRequiredService<IUnitOfWork>();
        
        var emailString = "alli@via.dk";
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init()
            .WithId(guestId.ToString())
            .WithEmail(emailString)
            .WithFirstName("Alice")
            .WithLastName("Liddell")
            .WithProfileUrl("https://ex.com/p.png")
            .Build();
    
        await gRepo.AddAsync(guest);
        await uow.SaveChangesAsync();
    
        // new scope to ensure a fresh DbContext
        using var scope2 = _fx.Provider.CreateScope();
        var sp2 = scope2.ServiceProvider;
    
        var gRepo2 = sp2.GetRequiredService<IGuestRepository>();
        var loaded = await gRepo2.GetAsync(guest.Id!);
    
        Assert.NotNull(loaded);
        Assert.Equal(emailString, loaded!.Email.Value);
    }

}