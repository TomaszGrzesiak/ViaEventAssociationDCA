using System.Collections.Concurrent;
using System.Text;

namespace ViaEventAssociation.Core.Application.Logger;

public class FileLogger : ILogger
{
    // One lock object per path, shared across all FileLogger instances
    private static readonly ConcurrentDictionary<string, object> PathLocks = new();

    private readonly string _path;
    private readonly object _lock;

    public FileLogger(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));

        // Ensure all loggers targeting the same file share the same lock
        _lock = PathLocks.GetOrAdd(_path, _ => new object());
    }

    public void Info(string message) => Write("INFO", message, null);

    public void Error(string message, Exception ex) => Write("ERROR", message, ex);

    private void Write(string level, string message, Exception? ex)
    {
        var line = $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}] {level} {message}";
        if (ex != null)
        {
            line += Environment.NewLine + ex;
        }

        try
        {
            lock (_lock)
            {
                var directory = Path.GetDirectoryName(_path);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.AppendAllText(_path, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch (IOException)
        {
            // Do NOT let logging kill the app.
            // In tests / high-concurrency cases the file might be temporarily locked.
            // We silently ignore IO errors.
        }
        catch (UnauthorizedAccessException)
        {
            // Same story: if we don't have permission, log failure should not break commands/queries.
        }
    }
}