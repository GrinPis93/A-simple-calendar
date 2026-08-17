using System.Collections.ObjectModel;
using System.Globalization;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASimpleCalendar.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private static readonly ThemeOption[] ThemeOptions =
    {
        new("Тёмная", ThemeMode.Dark),
        new("Светлая", ThemeMode.Light),
        new("Авто (по системе)", ThemeMode.Auto)
    };

    private readonly WidgetService _widgets;
    private readonly ISettingsRepository _settings;
    private readonly AutoStartService _autoStart;
    private readonly CategoryService _categoryService;

    [ObservableProperty]
    private ThemeOption _selectedThemeOption = ThemeOptions[0];

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

    public ObservableCollection<CategoryItem> Categories { get; } = new();

    public SettingsViewModel(WidgetService widgets, ISettingsRepository settings, AutoStartService autoStart, CategoryService categoryService)
    {
        _widgets = widgets;
        _settings = settings;
        _autoStart = autoStart;
        _categoryService = categoryService;

        var mode = ThemeHelper.Parse(settings.Get("theme"));
        _selectedThemeOption = ThemeOptions.First(o => o.Value == mode);
        _clockEnabled = widgets.IsClockOpen;
        _calendarEnabled = widgets.IsCalendarOpen;
        _tasksEnabled = widgets.IsTasksOpen;
        _autoStartEnabled = autoStart.IsEnabled();
        _widgetOpacity = widgets.Opacity;
        _widgetScale = widgets.Scale;
        _widgetTopmost = widgets.Topmost;
        _widgetLocked = widgets.Locked;

        ReloadCategories();
    }

    public void ReloadCategories()
    {
        Categories.Clear();
        foreach (var category in _categoryService.GetCategories())
        {
            Categories.Add(category);
        }
    }

    public void AddCategory(CategoryItem item)
    {
        var list = _categoryService.GetCategories();
        list.Add(item);
        _categoryService.SaveCategories(list);
        ReloadCategories();
    }

    public void RemoveCategory(CategoryItem item)
    {
        var list = _categoryService.GetCategories();
        list.RemoveAll(c => c.Name == item.Name);
        _categoryService.SaveCategories(list);
        ReloadCategories();
    }

    public IReadOnlyList<ThemeOption> ThemeChoices => ThemeOptions;

    partial void OnSelectedThemeOptionChanged(ThemeOption value)
    {
        ThemeHelper.Apply(value.Value);
        _settings.Set("theme", ThemeHelper.ToString(value.Value));
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
