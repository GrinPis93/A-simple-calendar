using System.Globalization;
using System.Windows;
using ASimpleCalendar.Data;
using ASimpleCalendar.Widgets;

namespace ASimpleCalendar.Services;

public class WidgetService
{
    private readonly ISettingsRepository _settings;

    private ClockWidget? _clock;
    private MiniCalendarWidget? _calendar;
    private TasksWidget? _tasks;

    public WidgetService(ISettingsRepository settings)
    {
        _settings = settings;
    }

    public bool IsClockOpen => _clock is not null;
    public bool IsCalendarOpen => _calendar is not null;
    public bool IsTasksOpen => _tasks is not null;

    public double Opacity => ReadDouble("widget.opacity", 1.0);
    public double Scale => ReadDouble("widget.scale", 1.0);
    public bool Topmost => _settings.Get("widget.topmost") != "false";
    public bool Locked => _settings.Get("widget.lock") == "true";

    public void ToggleClock()
    {
        if (_clock is not null)
        {
            _clock.Close();
            return;
        }

        _clock = new ClockWidget();
        ApplyTo(_clock, "widget.clock");
        ApplyPosition(_clock, "widget.clock");
        _clock.Closed += (_, _) =>
        {
            SavePosition(_clock, "widget.clock");
            SaveSize(_clock, "widget.clock");
            _clock = null;
        };
        _clock.Show();
    }

    public void ToggleCalendar()
    {
        if (_calendar is not null)
        {
            _calendar.Close();
            return;
        }

        _calendar = new MiniCalendarWidget();
        ApplyTo(_calendar, "widget.calendar");
        ApplyPosition(_calendar, "widget.calendar");
        _calendar.Closed += (_, _) =>
        {
            SavePosition(_calendar, "widget.calendar");
            SaveSize(_calendar, "widget.calendar");
            _calendar = null;
        };
        _calendar.Show();
    }

    public void ToggleTasks()
    {
        if (_tasks is not null)
        {
            _tasks.Close();
            return;
        }

        _tasks = new TasksWidget();
        ApplyTo(_tasks, "widget.tasks");
        ApplyPosition(_tasks, "widget.tasks");
        _tasks.Closed += (_, _) =>
        {
            SavePosition(_tasks, "widget.tasks");
            SaveSize(_tasks, "widget.tasks");
            _tasks = null;
        };
        _tasks.Show();
    }

    public void ApplySettings()
    {
        ApplySettingsTo(_clock, "widget.clock");
        ApplySettingsTo(_calendar, "widget.calendar");
        ApplySettingsTo(_tasks, "widget.tasks");
    }

    private void ApplyTo(Window window, string key)
    {
        window.Opacity = Opacity;
        window.Topmost = Topmost;

        var savedWidth = ReadDouble(key + ".width", 0);
        var savedHeight = ReadDouble(key + ".height", 0);

        if (savedWidth >= 100 && savedHeight >= 80)
        {
            window.Width = savedWidth;
            window.Height = savedHeight;
        }
        else
        {
            window.Width = BaseWidth(window) * Scale;
            window.Height = BaseHeight(window) * Scale;
        }

        if (window is IWidget widget)
        {
            widget.LockDrag = Locked;
        }
    }

    private void ApplySettingsTo(Window? window, string key)
    {
        if (window is null)
        {
            return;
        }

        window.Opacity = Opacity;
        window.Topmost = Topmost;
        window.Width = BaseWidth(window) * Scale;
        window.Height = BaseHeight(window) * Scale;

        if (window is IWidget widget)
        {
            widget.LockDrag = Locked;
        }

        SaveSize(window, key);
    }

    private static double BaseWidth(Window window) => window is ClockWidget ? 250 : 300;

    private static double BaseHeight(Window window) => window is ClockWidget ? 130 : 330;

    private void ApplyPosition(Window window, string key)
    {
        var culture = CultureInfo.InvariantCulture;
        var leftText = _settings.Get(key + ".left");
        var topText = _settings.Get(key + ".top");

        if (double.TryParse(leftText, NumberStyles.Any, culture, out var left) &&
            double.TryParse(topText, NumberStyles.Any, culture, out var top) &&
            left >= SystemParameters.VirtualScreenLeft &&
            top >= SystemParameters.VirtualScreenTop)
        {
            window.Left = left;
            window.Top = top;
        }
    }

    private void SavePosition(Window window, string key)
    {
        var culture = CultureInfo.InvariantCulture;
        _settings.Set(key + ".left", window.Left.ToString(culture));
        _settings.Set(key + ".top", window.Top.ToString(culture));
    }

    private void SaveSize(Window window, string key)
    {
        var culture = CultureInfo.InvariantCulture;
        _settings.Set(key + ".width", window.Width.ToString(culture));
        _settings.Set(key + ".height", window.Height.ToString(culture));
    }

    private double ReadDouble(string key, double fallback)
    {
        var value = _settings.Get(key);
        return value is not null &&
               double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;
    }
}
