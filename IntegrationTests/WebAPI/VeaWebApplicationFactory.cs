using EfcDmPersistence;
using EfcQueries.Models;
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
    private IServiceCollection serviceCollection;

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
            services.AddDbContext<VeaReadModelContext>(options => { options.UseSqlite(connString); });

            services.AddScoped<ISystemTime, FakeSystemTime>();

            SetupCleanDatabase(services);
        });
    }
    
    private void SetupCleanDatabase(IServiceCollection services)
    {
        DmContext dmContext = services.BuildServiceProvider().GetService<DmContext>()!;
        dmContext.Database.EnsureDeleted();
        dmContext.Database.EnsureCreated();
        // could seed database here?
    }

    private string GetConnectionString()
    {
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        return "Data Source = " + testDbName;
    }

    protected override void Dispose(bool disposing)
    {
        // clean up the database
        DmContext dmContext = serviceCollection.BuildServiceProvider().GetService<DmContext>()!;
        dmContext.Database.EnsureDeleted();
        base.Dispose(disposing);
    }

}