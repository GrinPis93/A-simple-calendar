using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ASimpleCalendar.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Widgets;

public partial class TasksWidget : Window
{
    private readonly DispatcherTimer _timer;

    public TasksWidget()
    {
        InitializeComponent();

        Left = SystemParameters.WorkArea.Right - Width - 24;
        Top = SystemParameters.WorkArea.Top + 520;

        Reload();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        _timer.Tick += (_, _) => Reload();
        _timer.Start();
    }

    private void Reload()
    {
        ItemsPanel.Children.Clear();

        var events = App.Services.GetRequiredService<IEventRepository>()
            .GetByRange(DateTime.Today, DateTime.Today.AddDays(1));

        var reminders = App.Services.GetRequiredService<IReminderRepository>()
            .GetActive()
            .Where(r => r.RemindAt.Date == DateTime.Today);

        if (events.Count == 0 && !reminders.Any())
        {
            ItemsPanel.Children.Add(new TextBlock
            {
                Text = "На сегодня ничего не запланировано",
                Foreground = new SolidColorBrush(Color.FromArgb(0x99, 0xFF, 0xFF, 0xFF)),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            });
            return;
        }

        foreach (var ev in events.OrderBy(e => e.StartDate))
        {
            ItemsPanel.Children.Add(BuildItem($"{ev.StartDate:HH:mm}  {ev.Title}", ev.Color));
        }

        foreach (var reminder in reminders.OrderBy(r => r.RemindAt))
        {
            ItemsPanel.Children.Add(BuildItem($"Напомин. {reminder.RemindAt:HH:mm}  {reminder.Title}", null));
        }
    }

    private static Border BuildItem(string text, string? colorHex)
    {
        var color = colorHex is null
            ? Color.FromRgb(0xE8, 0x80, 0x2A)
            : ParseColor(colorHex);

        return new Border
        {
            CornerRadius = new CornerRadius(6),
            Background = new SolidColorBrush(Color.FromArgb(0x1A, 0xFF, 0xFF, 0xFF)),
            Padding = new Thickness(8, 6, 8, 6),
            Margin = new Thickness(0, 0, 0, 6),
            Child = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Border
                    {
                        Width = 8,
                        Height = 8,
                        CornerRadius = new CornerRadius(4),
                        Background = new SolidColorBrush(color),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 8, 0)
                    },
                    new TextBlock
                    {
                        Text = text,
                        Foreground = Brushes.White,
                        FontSize = 12,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };
    }

    private static Color ParseColor(string hex)
    {
        try
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }
        catch
        {
            return Color.FromRgb(100, 116, 139);
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
