using EfcDmPersistence;
using EfcQueries.Models;
using EfcQueries.SeedFactories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Contracts;

namespace IntegrationTests.WebAPI;

internal class VeaWebApplicationFactory : WebApplicationFactory<Program>
{
    private IServiceCollection? serviceCollection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // setup extra test services.
        builder.ConfigureTestServices(services =>
        {
            serviceCollection = services;
            // Remove the existing DbContexts and Options
            services.RemoveAll(typeof(DbContextOptions<DmContext>));
            services.RemoveAll(typeof(DbContextOptions<VeaReadModelContext>));
            services.RemoveAll<DmContext>();
            services.RemoveAll<VeaReadModelContext>();

            string connString = GetConnectionString();
            services.AddDbContext<DmContext>(options => { options.UseSqlite(connString); });
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<DmContext>());
            services.AddDbContext<VeaReadModelContext>(options => { options.UseSqlite(connString); });

            services.RemoveAll<ISystemTime>();
            services.AddScoped<ISystemTime, FakeSystemTime>();

            SetupCleanDatabase(services);
        });
    }
    
    private void SetupCleanDatabase(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        
        var dmContext = scope.ServiceProvider.GetRequiredService<DmContext>();
        dmContext.Database.EnsureDeleted();
        dmContext.Database.EnsureCreated();

        // Seeding DB
        var veaReadModelContext = scope.ServiceProvider.GetRequiredService<VeaReadModelContext>();
        veaReadModelContext.Guests.AddRange(GuestSeedFactory.CreateGuests());
        veaReadModelContext.VeaEvents.AddRange(EventSeedFactory.CreateEvents());
        veaReadModelContext.SaveChanges();
    }

    private string GetConnectionString()
    {
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        return "Data Source = " + testDbName;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && serviceCollection is not null)
        {
            // clean up the database
            DmContext dmContext = serviceCollection.BuildServiceProvider().GetService<DmContext>()!;
            dmContext.Database.EnsureDeleted();
        }
        
        base.Dispose(disposing);
    }

}