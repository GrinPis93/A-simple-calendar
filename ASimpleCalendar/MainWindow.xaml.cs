using System.Linq;
using System.Windows;
using ASimpleCalendar.Views;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ASimpleCalendar;

public partial class MainWindow : FluentWindow
{
    private readonly System.Collections.Generic.Dictionary<string, System.Windows.Controls.UserControl> _pages = new();

    public MainWindow()
    {
        InitializeComponent();

        _pages["calendar"] = new CalendarView();
        _pages["notes"] = new NotesView();
        _pages["reminders"] = new RemindersView();
        _pages["settings"] = new SettingsView();

        var first = RootNavigation.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
        RootNavigation.SetCurrentValue(NavigationView.SelectedItemProperty, first);
        MainContent.Content = _pages["calendar"];
    }

    private void RootNavigation_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (RootNavigation.SelectedItem is NavigationViewItem { Tag: string tag } &&
            _pages.TryGetValue(tag, out var page))
        {
            MainContent.Content = page;
        }
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var current = ApplicationThemeManager.GetAppTheme();
        var next = current == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark;

        ApplicationThemeManager.Apply(next);
        ThemeButton.Icon = new SymbolIcon(next == ApplicationTheme.Dark
            ? SymbolRegular.WeatherMoon24
            : SymbolRegular.WeatherSunny24);
    }
}
