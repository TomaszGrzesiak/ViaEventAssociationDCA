using System.IO;
using EfcQueries;
using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helpers;

public static class ReadModelTestHelpers
{
   public static VeaReadModelContext SetupReadContext()
    {
        // Use OS temp directory to avoid permission / path issues
        string dbName = $"ReadModelTest_{Guid.NewGuid():N}.db";
        string dbPath = Path.Combine(Path.GetTempPath(), dbName);

        var optionsBuilder = new DbContextOptionsBuilder<VeaReadModelContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        var context = new VeaReadModelContext(optionsBuilder.Options);

        // For tests: start from a clean slate
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}