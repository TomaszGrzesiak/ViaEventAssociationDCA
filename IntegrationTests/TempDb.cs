namespace IntegrationTests;

public static class TempDb
{
    // Creates a unique file-based SQLite path that persists across DbContext instances.
    public static string NewFileDbPath(string prefix = "vea_tests")
    {
        var name = $"{prefix}_{Guid.NewGuid():N}.db";
        return Path.Combine(Path.GetTempPath(), name);
    }
}