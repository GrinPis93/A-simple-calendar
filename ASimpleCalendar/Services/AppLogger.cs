using System.IO;

namespace ASimpleCalendar.Services;

public static class AppLogger
{
    private static readonly object Lock = new();

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
            }
        }
        catch
        {
            // логирование не должно ронять приложение
        }
    }
}
