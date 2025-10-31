using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EfcDmPersistence;

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<DmContext>
{
    public DmContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DmContext>();
        optionsBuilder.UseSqlite("Data source=vea_write.db");
        return new DmContext(optionsBuilder.Options);
    }
    
    
    private static async Task SaveAndClearAsync<T>(T entity, DmContext context) 
        where T : class
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
}
