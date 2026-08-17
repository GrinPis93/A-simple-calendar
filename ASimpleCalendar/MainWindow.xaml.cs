using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        foreach (var item in RootNavigation.MenuItems.OfType<NavigationViewItem>())
        {
            WireItem(item);
        }

        foreach (var item in RootNavigation.FooterMenuItems.OfType<NavigationViewItem>())
        {
            WireItem(item);
        }

        var first = RootNavigation.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
        if (first is not null)
        {
            first.IsActive = true;
        }

        ShowPage("calendar");
    }

    private void WireItem(NavigationViewItem item)
    {
        item.AddHandler(
            MouseLeftButtonUpEvent,
            new MouseButtonEventHandler((sender, _) =>
            {
                if (sender is NavigationViewItem nav && nav.Tag is string tag)
                {
                    ShowPage(tag);
                }
            }),
            handledEventsToo: true);
    }

    private void ShowPage(string tag)
    {
        if (_pages.TryGetValue(tag, out var page))
        {
            MainContent.Content = page;
        }
    }

    public void ShowMain()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    public void ShowNotesAndCreate()
    {
        ShowPage("notes");
        ((NotesView)_pages["notes"]).CreateNote();
    }

    public void ShowRemindersAndCreate()
    {
        ShowPage("reminders");
        ((RemindersView)_pages["reminders"]).CreateReminder();
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
}
