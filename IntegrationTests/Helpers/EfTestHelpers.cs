using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Helpers;

public static class EfTestHelpers
{
    public static async Task SaveAndClearAsync(object entity, DbContext ctx)
    {
        await ctx.AddAsync(entity);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();
    }
}