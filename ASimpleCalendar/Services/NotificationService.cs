using Microsoft.Toolkit.Uwp.Notifications;

namespace ASimpleCalendar.Services;

public class NotificationService
{
    public void ShowToast(string title, string message)
    {
        try
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
        catch
        {
            // Уведомления не должны ронять приложение, если Toast недоступен.
        }
    }
}
