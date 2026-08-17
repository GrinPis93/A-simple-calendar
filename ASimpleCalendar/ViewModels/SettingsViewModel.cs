using System.Globalization;
using ASimpleCalendar.Data;
using ASimpleCalendar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ASimpleCalendar.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly WidgetService _widgets;
    private readonly ISettingsRepository _settings;
    private readonly AutoStartService _autoStart;

    [ObservableProperty]
    private bool _isDarkTheme;

    [ObservableProperty]
    private bool _clockEnabled;

    [ObservableProperty]
    private bool _calendarEnabled;

    [ObservableProperty]
    private bool _tasksEnabled;

    [ObservableProperty]
    private bool _autoStartEnabled;

    [ObservableProperty]
    private double _widgetOpacity;

    [ObservableProperty]
    private double _widgetScale;

    [ObservableProperty]
    private bool _widgetTopmost;

    [ObservableProperty]
    private bool _widgetLocked;

    public SettingsViewModel(WidgetService widgets, ISettingsRepository settings, AutoStartService autoStart)
    {
        _widgets = widgets;
        _settings = settings;
        _autoStart = autoStart;

        _isDarkTheme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark;
        _clockEnabled = widgets.IsClockOpen;
        _calendarEnabled = widgets.IsCalendarOpen;
        _tasksEnabled = widgets.IsTasksOpen;
        _autoStartEnabled = autoStart.IsEnabled();
        _widgetOpacity = widgets.Opacity;
        _widgetScale = widgets.Scale;
        _widgetTopmost = widgets.Topmost;
        _widgetLocked = widgets.Locked;
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
        _settings.Set("theme", value ? "dark" : "light");
    }

    partial void OnClockEnabledChanged(bool value) => _widgets.ToggleClock();

    partial void OnCalendarEnabledChanged(bool value) => _widgets.ToggleCalendar();

    partial void OnTasksEnabledChanged(bool value) => _widgets.ToggleTasks();

    partial void OnAutoStartEnabledChanged(bool value) => _autoStart.SetEnabled(value);

    partial void OnWidgetOpacityChanged(double value)
    {
        _settings.Set("widget.opacity", value.ToString(CultureInfo.InvariantCulture));
        _widgets.ApplySettings();
    }

    partial void OnWidgetScaleChanged(double value)
    {
        _settings.Set("widget.scale", value.ToString(CultureInfo.InvariantCulture));
        _widgets.ApplySettings();
    }

    partial void OnWidgetTopmostChanged(bool value)
    {
        _settings.Set("widget.topmost", value ? "true" : "false");
        _widgets.ApplySettings();
    }

    partial void OnWidgetLockedChanged(bool value)
    {
        _settings.Set("widget.lock", value ? "true" : "false");
        _widgets.ApplySettings();
    }
}
