using System.Text;

namespace ViaEventAssociation.Core.Application.Logger;

public class FileLogger : ILogger
{
    private readonly string _path;
    private readonly Lock _lock = new();

    public FileLogger(string path) => _path = path;

    public void Info(string message) => Write("INFO", message, null);
    public void Error(string message, Exception ex) => Write("ERROR", message, ex);

    private void Write(string level, string message, Exception? ex)
    {
        var line = $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}] {level} {message}";
        if (ex != null) line += Environment.NewLine + ex;

        lock (_lock)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.AppendAllText(_path, line + Environment.NewLine, Encoding.UTF8);
        }
    }
}
