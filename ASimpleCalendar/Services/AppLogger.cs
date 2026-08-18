using System.IO;

namespace ASimpleCalendar.Services;

public static class AppLogger
{
    private static readonly object Lock = new();
    private const long MaxLogSize = 1_000_000;

    public static string LogPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ASimpleCalendar",
        "error.log");

    public static void Log(string message, Exception? exception = null)
    {
        try
        {
            var directory = Path.GetDirectoryName(LogPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            if (exception is not null)
            {
                line += Environment.NewLine + exception;
            }

            lock (Lock)
            {
                File.AppendAllText(LogPath, line + Environment.NewLine);
                TruncateIfNeeded();
            }
        }
        catch
        {
            // логирование не должно ронять приложение
        }
    }

    private static void TruncateIfNeeded()
    {
        try
        {
            var info = new FileInfo(LogPath);
            if (!info.Exists || info.Length <= MaxLogSize)
            {
                return;
            }

            var text = File.ReadAllText(LogPath);
            var keepFrom = text.Length / 2;
            File.WriteAllText(LogPath, text[keepFrom..]);
        }
        catch
        {
            // игнорируем — обрезка не критична
        }
    }
}
