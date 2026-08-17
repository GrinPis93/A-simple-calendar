using System.Windows.Threading;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Services;

public class ReminderScheduler
{
    private readonly IReminderRepository _reminders;
    private readonly NotificationService _notifications;
    private readonly DispatcherTimer _timer;

    public ReminderScheduler(IReminderRepository reminders, NotificationService notifications)
    {
        _reminders = reminders;
        _notifications = notifications;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;

        foreach (var reminder in _reminders.GetActive())
        {
            if (reminder.RemindAt > now)
            {
                continue;
            }

            if (reminder.LastNotifiedAt is not null && reminder.LastNotifiedAt >= reminder.RemindAt)
            {
                continue;
            }

            _notifications.ShowToast(reminder.Title, reminder.Message ?? "Напоминание");

            reminder.LastNotifiedAt = now;
            if (reminder.Repeat != RepeatRule.None)
            {
                reminder.RemindAt = NextOccurrence(reminder.RemindAt, reminder.Repeat);
            }
            else
            {
                reminder.IsActive = false;
            }

            _reminders.Update(reminder);
        }
    }

    private static DateTime NextOccurrence(DateTime current, RepeatRule repeat) => repeat switch
    {
        RepeatRule.Daily => current.AddDays(1),
        RepeatRule.Weekly => current.AddDays(7),
        RepeatRule.Monthly => current.AddMonths(1),
        RepeatRule.Yearly => current.AddYears(1),
        _ => current
    };
}
