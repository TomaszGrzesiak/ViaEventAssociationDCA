using EfcDmPersistence;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests;

public class GlobalUsings
{
    public static DmContext SetupContext()
    {
        DbContextOptionsBuilder<DmContext> optionsBuilder = new();
        string testDbName = "Test" + Guid.NewGuid() +".db";
        optionsBuilder.UseSqlite(@"Data Source = " + testDbName);
        DmContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }
}
