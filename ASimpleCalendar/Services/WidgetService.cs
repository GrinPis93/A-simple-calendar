using System.Windows;
using System.Windows.Media;
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
        ApplyTo(_clock);
        _clock.Closed += (_, _) => _clock = null;
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
        ApplyTo(_calendar);
        _calendar.Closed += (_, _) => _calendar = null;
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
        ApplyTo(_tasks);
        _tasks.Closed += (_, _) => _tasks = null;
        _tasks.Show();
    }

    public void ApplySettings()
    {
        ApplyTo(_clock);
        ApplyTo(_calendar);
        ApplyTo(_tasks);
    }

    private void ApplyTo(Window? window)
    {
        if (window is null)
        {
            return;
        }

        window.Opacity = Opacity;
        window.Topmost = Topmost;
        window.LayoutTransform = new ScaleTransform(Scale, Scale);

        if (window is IWidget widget)
        {
            widget.LockDrag = Locked;
        }
    }

    private double ReadDouble(string key, double fallback)
    {
        var value = _settings.Get(key);
        return value is not null &&
               double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;
    }
}
