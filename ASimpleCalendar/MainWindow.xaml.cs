using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ASimpleCalendar.Views;
using Wpf.Ui.Controls;

namespace ASimpleCalendar;

public partial class MainWindow : FluentWindow
{
    private readonly Dictionary<string, UserControl> _pages = new();

    public MainWindow()
    {
        InitializeComponent();

        _pages["calendar"] = new CalendarView();
        _pages["notes"] = new NotesView();
        _pages["reminders"] = new RemindersView();
        _pages["settings"] = new SettingsView();

        var first = RootNavigation.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
        if (first is not null)
        {
            first.IsActive = true;
        }

        MainContent.Content = _pages["calendar"];
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (!App.IsShuttingDown)
        {
            e.Cancel = true;
            Hide();
        }
    }

    private void RootNavigation_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (RootNavigation.SelectedItem is NavigationViewItem { Tag: string tag } &&
            _pages.TryGetValue(tag, out var page))
        {
            MainContent.Content = page;
        }
    }
}
