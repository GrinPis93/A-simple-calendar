using System.Windows;
using ASimpleCalendar.Widgets;

namespace ASimpleCalendar.Services;

public class WidgetService
{
    private ClockWidget? _clock;
    private MiniCalendarWidget? _calendar;
    private TasksWidget? _tasks;

    public bool IsClockOpen => _clock is not null;
    public bool IsCalendarOpen => _calendar is not null;
    public bool IsTasksOpen => _tasks is not null;

    public void ToggleClock()
    {
        if (_clock is not null)
        {
            _clock.Close();
            return;
        }

        _clock = new ClockWidget();
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
        _tasks.Closed += (_, _) => _tasks = null;
        _tasks.Show();
    }
}
