using System.Linq;
using System.Windows;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Views;

public partial class ReminderDialog : Window
{
    private static readonly RepeatOption[] RepeatOptions =
    {
        new("Без повтора", RepeatRule.None),
        new("Ежедневно", RepeatRule.Daily),
        new("Еженедельно", RepeatRule.Weekly),
        new("Ежемесячно", RepeatRule.Monthly),
        new("Ежегодно", RepeatRule.Yearly)
    };

    public Reminder? Result { get; private set; }

    public ReminderDialog(Reminder? existing = null)
    {
        InitializeComponent();
        RepeatBox.ItemsSource = RepeatOptions;
        RepeatBox.SelectedIndex = 0;
        ActiveBox.IsChecked = true;

        var events = App.Services.GetRequiredService<IEventRepository>().GetAll();
        EventBox.ItemsSource = events;

        if (existing is not null)
        {
            TitleBox.Text = existing.Title;
            MessageBox.Text = existing.Message ?? string.Empty;
            DatePicker.SelectedDate = existing.RemindAt.Date;
            TimeBox.Text = existing.RemindAt.ToString("HH:mm");
            ActiveBox.IsChecked = existing.IsActive;
            RepeatBox.SelectedItem = RepeatOptions.FirstOrDefault(o => o.Value == existing.Repeat);

            if (existing.EventId is int eventId)
            {
                EventBox.SelectedItem = events.FirstOrDefault(e => e.Id == eventId);
            }
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            System.Windows.MessageBox.Show("Введите заголовок напоминания.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (DatePicker.SelectedDate is not DateTime date)
        {
            System.Windows.MessageBox.Show("Выберите дату.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var time = TimeSpan.TryParse(TimeBox.Text.Trim(), out var parsed) ? parsed : TimeSpan.Zero;

        Result = new Reminder
        {
            Title = TitleBox.Text.Trim(),
            Message = string.IsNullOrWhiteSpace(MessageBox.Text) ? null : MessageBox.Text.Trim(),
            RemindAt = date.Date + time,
            Repeat = (RepeatBox.SelectedItem as RepeatOption)?.Value ?? RepeatRule.None,
            EventId = (EventBox.SelectedItem as Event)?.Id,
            IsActive = ActiveBox.IsChecked == true,
            LastNotifiedAt = null
        };

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
