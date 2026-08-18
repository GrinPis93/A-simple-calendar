using System.Linq;
using System.Windows;
using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Views;

public partial class EventDialog : Window
{
    private static readonly RemindOption[] RemindOptions =
    {
        new("Не напоминать", 0),
        new("За 5 минут", 5),
        new("За 15 минут", 15),
        new("За 30 минут", 30),
        new("За 1 час", 60),
        new("За 1 день", 1440)
    };

    private record RemindOption(string Name, int Minutes);

    public Event? Result { get; private set; }

    public EventDialog(Event? existing = null, DateTime? initialDate = null)
    {
        InitializeComponent();
        CategoryBox.ItemsSource = App.Services.GetRequiredService<CategoryService>().GetCategories();
        RepeatBox.ItemsSource = RepeatOptionList.All;
        RepeatBox.SelectedIndex = 0;
        RemindBox.ItemsSource = RemindOptions;
        RemindBox.SelectedIndex = 0;

        if (existing is not null)
        {
            TitleBox.Text = existing.Title;
            DescriptionBox.Text = existing.Description ?? string.Empty;
            DatePicker.SelectedDate = existing.StartDate.Date;
            StartTimeBox.Text = existing.StartDate.ToString("HH:mm");
            EndTimeBox.Text = existing.EndDate?.ToString("HH:mm") ?? string.Empty;
            AllDayBox.IsChecked = existing.AllDay;
            RepeatUntilPicker.SelectedDate = existing.RepeatUntil;

            if (existing.Color is not null)
            {
                CategoryBox.SelectedItem = CategoryBox.Items.OfType<CategoryItem>().FirstOrDefault(c => c.Color == existing.Color);
            }

            RepeatBox.SelectedItem = RepeatOptionList.All.FirstOrDefault(o => o.Value == existing.Repeat);
            RemindBox.SelectedItem = RemindOptions.FirstOrDefault(o => o.Minutes == existing.RemindBeforeMinutes);
        }
        else
        {
            var date = initialDate ?? DateTime.Today;
            DatePicker.SelectedDate = date;
            StartTimeBox.Text = "09:00";
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            MessageBox.Show("Введите название события.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (DatePicker.SelectedDate is not DateTime date)
        {
            MessageBox.Show("Выберите дату.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var start = date.Date + ParseTime(StartTimeBox.Text);

        DateTime? end = null;
        if (!string.IsNullOrWhiteSpace(EndTimeBox.Text))
        {
            end = date.Date + ParseTime(EndTimeBox.Text);
        }

        var category = CategoryBox.SelectedItem as CategoryItem;

        Result = new Event
        {
            Title = TitleBox.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim(),
            StartDate = start,
            EndDate = end,
            AllDay = AllDayBox.IsChecked == true,
            Category = category?.Name,
            Color = category?.Color,
            Repeat = (RepeatBox.SelectedItem as RepeatOption)?.Value ?? RepeatRule.None,
            RepeatUntil = RepeatUntilPicker.SelectedDate,
            RemindBeforeMinutes = (RemindBox.SelectedItem as RemindOption)?.Minutes ?? 0,
            CreatedAt = DateTime.Now
        };

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static TimeSpan ParseTime(string text)
    {
        return TimeSpan.TryParse(text.Trim(), out var time) ? time : TimeSpan.Zero;
    }
}
