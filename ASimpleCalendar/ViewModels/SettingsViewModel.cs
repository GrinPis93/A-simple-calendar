using ASimpleCalendar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ASimpleCalendar.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly WidgetService _widgets;

    [ObservableProperty]
    private bool _isDarkTheme;

    [ObservableProperty]
    private bool _clockEnabled;

    [ObservableProperty]
    private bool _calendarEnabled;

    [ObservableProperty]
    private bool _tasksEnabled;

    public SettingsViewModel(WidgetService widgets)
    {
        _widgets = widgets;
        _isDarkTheme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark;
        _clockEnabled = widgets.IsClockOpen;
        _calendarEnabled = widgets.IsCalendarOpen;
        _tasksEnabled = widgets.IsTasksOpen;
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
    }

    partial void OnClockEnabledChanged(bool value) => _widgets.ToggleClock();

    partial void OnCalendarEnabledChanged(bool value) => _widgets.ToggleCalendar();

    partial void OnTasksEnabledChanged(bool value) => _widgets.ToggleTasks();
}
