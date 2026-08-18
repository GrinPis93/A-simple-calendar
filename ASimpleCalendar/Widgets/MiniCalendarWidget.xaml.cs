using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASimpleCalendar.Widgets;

public partial class MiniCalendarWidget : Window, IWidget
{
    private DateTime _shownMonth;

    public bool LockDrag { get; set; }

    public MiniCalendarWidget()
    {
        InitializeComponent();

        Left = SystemParameters.WorkArea.Right - Width - 24;
        Top = SystemParameters.WorkArea.Top + 170;

        Build();

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        timer.Tick += (_, _) =>
        {
            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (currentMonth != _shownMonth)
            {
                Build();
            }
        };
        timer.Start();

        Closed += (_, _) => timer.Stop();
    }

    private void Build()
    {
        var ru = CultureInfo.GetCultureInfo("ru-RU");
        var today = DateTime.Today;
        _shownMonth = new DateTime(today.Year, today.Month, 1);
        MonthText.Text = ru.TextInfo.ToTitleCase(today.ToString("MMMM yyyy", ru));

        var first = new DateTime(today.Year, today.Month, 1);
        var offset = ((int)first.DayOfWeek + 6) % 7;
        var start = first.AddDays(-offset);

        DayGrid.Children.Clear();
        for (var i = 0; i < 42; i++)
        {
            var date = start.AddDays(i);
            var text = new TextBlock
            {
                Text = date.Day.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12
            };

            if (date == today)
            {
                text.Foreground = new SolidColorBrush(Color.FromRgb(0x4F, 0x6B, 0xED));
                text.FontWeight = FontWeights.Bold;
            }
            else if (date.Month != today.Month)
            {
                text.Foreground = new SolidColorBrush(Color.FromArgb(0x55, 0xFF, 0xFF, 0xFF));
            }
            else
            {
                text.Foreground = Brushes.White;
            }

            DayGrid.Children.Add(text);
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (LockDrag)
        {
            return;
        }

        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void ResizeGrip_DragDelta(object sender, DragDeltaEventArgs e)
    {
        var newWidth = Width + e.HorizontalChange;
        var newHeight = Height + e.VerticalChange;
        if (newWidth >= 160)
        {
            Width = newWidth;
        }

        if (newHeight >= 160)
        {
            Height = newHeight;
        }
    }
}
