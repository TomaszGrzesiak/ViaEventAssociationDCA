using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EfcDmPersistence;
using EfcDmPersistence.GuestPersistence;
using EfcDmPersistence.UnitOfWork;
using EfcDmPersistence.VeaEventPersistence;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;

namespace IntegrationTests;

// Composition root for tests
public sealed class ServiceProviderFixture : IDisposable
{
    public ServiceProvider Provider { get; }
    private readonly string _dbPath;

    public ServiceProviderFixture()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"vea_tests_{Guid.NewGuid():N}.db");
        var connString = $"Data Source={_dbPath}";

        var services = new ServiceCollection();

        // 1) ONE AddDbContext with your REAL context type
        services.AddDbContext<DmContext>(o =>
        {
            o.UseSqlite(connString);
            // optional debugging:
            // o.EnableSensitiveDataLogging();
            // o.LogTo(Console.WriteLine);
        });

        // 2) Map base DbContext to concrete (your repos take DbContext)
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<DmContext>());

        // 3) UoW + repos as scoped
        services.AddScoped<IUnitOfWork, SqliteUnitOfWork>();
        services.AddScoped<IEventRepository, VeaEventSqliteRepository>();
        services.AddScoped<IGuestRepository, GuestSqliteRepository>();

        Provider = services.BuildServiceProvider(validateScopes: true);

        // 4) Create schema ONCE for this database file
        using var scope = Provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DmContext>();
        ctx.Database.EnsureDeleted();   // nuke leftover file from earlier runs
        ctx.Database.EnsureCreated();   // build tables for the current model
    }

    public void Dispose()
    {
        Provider.Dispose();
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { /* ignore */ }
    }
}
