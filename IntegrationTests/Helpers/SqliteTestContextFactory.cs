using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helpers;

public static class SqliteTestContextFactory
{
    public static VeaReadModelContext SetupReadContext()
    {
        DbContextOptionsBuilder<VeaReadModelContext> optionsBuilder = new();
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        optionsBuilder.UseSqlite(@"Data Source=" + testDbName);

        var context = new VeaReadModelContext(optionsBuilder.Options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}