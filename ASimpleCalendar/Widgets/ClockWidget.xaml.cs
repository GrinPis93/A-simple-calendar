using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ASimpleCalendar.Widgets;

public partial class ClockWidget : Window, IWidget
{
    private readonly DispatcherTimer _timer;

    public bool LockDrag { get; set; }

    public ClockWidget()
    {
        InitializeComponent();

        Left = SystemParameters.WorkArea.Right - Width - 24;
        Top = SystemParameters.WorkArea.Top + 24;

        UpdateClock();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (_, _) => UpdateClock();
        _timer.Start();

        Closed += (_, _) => _timer.Stop();
    }

    private void UpdateClock()
    {
        var ru = CultureInfo.GetCultureInfo("ru-RU");
        TimeText.Text = DateTime.Now.ToString("HH:mm:ss");
        DateText.Text = ru.TextInfo.ToTitleCase(DateTime.Now.ToString("dddd, d MMMM yyyy", ru));
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
}
