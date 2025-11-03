using EfcDmPersistence;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helpers;

public abstract class EfTestHelpers
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
    
    protected async Task AddAndSaveAndClearAsync(object entity, DbContext dbContext)
    {
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

}