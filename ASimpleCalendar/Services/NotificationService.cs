using ASimpleCalendar.Data;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ASimpleCalendar.Services;

public class NotificationService
{
    private readonly IReminderRepository _reminders;

    public NotificationService(IReminderRepository reminders)
    {
        _reminders = reminders;
        ToastNotificationManagerCompat.OnActivated += OnToastActivated;
    }

    public void ShowToast(string title, string message, int reminderId)
    {
        try
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .AddButton(new ToastButton()
                    .SetContent("Отложить на 10 минут")
                    .AddArgument("action", "snooze")
                    .AddArgument("minutes", "10"))
                .AddButton(new ToastButton()
                    .SetContent("Отложить на час")
                    .AddArgument("action", "snooze")
                    .AddArgument("minutes", "60"))
                .Show();
        }
        catch
        {
            // Уведомления не должны ронять приложение.
        }
    }

    private void OnToastActivated(ToastNotificationActivatedEventArgsCompat e)
    {
        var args = ToastArguments.Parse(e.Argument);

        if (args.TryGetValue("action", out var action) &&
            action == "snooze" &&
            args.TryGetValue("id", out var idText) &&
            int.TryParse(idText, out var id))
        {
            var minutes = args.TryGetValue("minutes", out var minutesText) &&
                          int.TryParse(minutesText, out var parsed)
                ? parsed
                : 10;

            var reminder = _reminders.GetById(id);
            if (reminder is not null)
            {
                reminder.RemindAt = DateTime.Now.AddMinutes(minutes);
                _reminders.Update(reminder);
            }
        }
    }
}
