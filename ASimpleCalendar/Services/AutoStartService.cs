using Microsoft.Win32;

namespace ASimpleCalendar.Services;

public class AutoStartService
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "ASimpleCalendar";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey);
        return key?.GetValue(AppName) is not null;
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);

        if (enabled)
        {
            var exePath = Environment.ProcessPath;
            if (exePath is not null)
            {
                key?.SetValue(AppName, $"\"{exePath}\"");
            }
        }
        else
        {
            key?.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }
}
