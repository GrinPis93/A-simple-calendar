using System.Windows.Threading;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Services;

public class ReminderScheduler
{
    private readonly IReminderRepository _reminders;
    private readonly IEventRepository _events;
    private readonly NotificationService _notifications;
    private readonly DispatcherTimer _timer;

    public ReminderScheduler(IReminderRepository reminders, IEventRepository events, NotificationService notifications)
    {
        _reminders = reminders;
        _events = events;
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

            _notifications.ShowToast(reminder.Title, reminder.Message ?? "Напоминание", reminder.Id);

            reminder.LastNotifiedAt = now;
            if (reminder.Repeat != RepeatRule.None)
            {
                reminder.RemindAt = EventOccurrenceService.NextOccurrence(reminder.RemindAt, reminder.Repeat);
            }
            else
            {
                reminder.IsActive = false;
            }

            _reminders.Update(reminder);
        }

        foreach (var ev in _events.GetPendingReminders())
        {
            if (ev.StartDate.AddMinutes(-ev.RemindBeforeMinutes) > now)
            {
                continue;
            }

            _notifications.ShowEventToast(ev.Title, $"Начало в {ev.StartDate:HH:mm}");

            ev.NotifiedAt = now;
            _events.Update(ev);
        }
    }
}
