using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ASimpleCalendar.Data;
using ASimpleCalendar.Views;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace ASimpleCalendar;

public partial class MainWindow : FluentWindow
{
    private readonly Dictionary<string, UserControl> _pages = new();

    public MainWindow()
    {
        InitializeComponent();

        LoadWindowBounds();

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
        if (!_pages.TryGetValue(tag, out var page))
        {
            return;
        }

        var item = FindItem(tag);
        if (item is not null)
        {
            SetActiveItem(item);
        }

        MainContent.Content = page;
    }

    private NavigationViewItem? FindItem(string tag)
    {
        return RootNavigation.MenuItems.OfType<NavigationViewItem>()
            .Concat(RootNavigation.FooterMenuItems.OfType<NavigationViewItem>())
            .FirstOrDefault(i => i.Tag is string t && t == tag);
    }

    private void SetActiveItem(NavigationViewItem active)
    {
        foreach (var item in RootNavigation.MenuItems.OfType<NavigationViewItem>())
        {
            item.IsActive = item == active;
        }

        foreach (var item in RootNavigation.FooterMenuItems.OfType<NavigationViewItem>())
        {
            item.IsActive = item == active;
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
        SaveWindowBounds();
        base.OnClosing(e);

        if (!App.IsShuttingDown)
        {
            e.Cancel = true;
            Hide();
        }
    }

    private void LoadWindowBounds()
    {
        var settings = App.Services.GetRequiredService<ISettingsRepository>();
        var culture = CultureInfo.InvariantCulture;

        if (double.TryParse(settings.Get("window.width"), NumberStyles.Any, culture, out var width) && width >= MinWidth)
        {
            Width = width;
        }

        if (double.TryParse(settings.Get("window.height"), NumberStyles.Any, culture, out var height) && height >= MinHeight)
        {
            Height = height;
        }

        if (double.TryParse(settings.Get("window.left"), NumberStyles.Any, culture, out var left) &&
            double.TryParse(settings.Get("window.top"), NumberStyles.Any, culture, out var top) &&
            left >= SystemParameters.VirtualScreenLeft &&
            top >= SystemParameters.VirtualScreenTop &&
            left <= SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth - 100 &&
            top <= SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight - 100)
        {
            Left = left;
            Top = top;
        }
    }

    private void SaveWindowBounds()
    {
        if (WindowState != WindowState.Normal)
        {
            return;
        }

        var settings = App.Services.GetRequiredService<ISettingsRepository>();
        var culture = CultureInfo.InvariantCulture;

        settings.Set("window.left", Left.ToString(culture));
        settings.Set("window.top", Top.ToString(culture));
        settings.Set("window.width", Width.ToString(culture));
        settings.Set("window.height", Height.ToString(culture));
    }
}
