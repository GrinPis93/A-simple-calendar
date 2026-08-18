using ASimpleCalendar.Models;
using Wpf.Ui.Appearance;

namespace ASimpleCalendar.Services;

public static class ThemeHelper
{
    public static void Apply(ThemeMode mode)
    {
        var theme = mode switch
        {
            ThemeMode.Dark => ApplicationTheme.Dark,
            ThemeMode.Light => ApplicationTheme.Light,
            _ => SystemThemeService.IsSystemDark() ? ApplicationTheme.Dark : ApplicationTheme.Light
        };

        ApplicationThemeManager.Apply(theme);
    }

    public static void ApplyAccent(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return;
        }

        try
        {
            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
            ApplicationAccentColorManager.Apply(color, ApplicationThemeManager.GetAppTheme());
        }
        catch
        {
            // некорректный цвет — игнорируем
        }
    }

    public static ThemeMode Parse(string? value) => value switch
    {
        "light" => ThemeMode.Light,
        "auto" => ThemeMode.Auto,
        _ => ThemeMode.Dark
    };

    public static string ToString(ThemeMode mode) => mode switch
    {
        ThemeMode.Light => "light",
        ThemeMode.Auto => "auto",
        _ => "dark"
    };
}
