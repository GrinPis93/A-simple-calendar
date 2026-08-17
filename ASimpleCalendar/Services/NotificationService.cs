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
                    .AddArgument("id", reminderId.ToString()))
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
            var reminder = _reminders.GetById(id);
            if (reminder is not null)
            {
                reminder.RemindAt = DateTime.Now.AddMinutes(10);
                _reminders.Update(reminder);
            }
        }
    }
}
