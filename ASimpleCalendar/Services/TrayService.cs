using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace ASimpleCalendar.Services;

public class TrayService
{
    private readonly Window _mainWindow;
    private readonly WidgetService _widgets;
    private readonly TaskbarIcon _icon;

    public TrayService(Window mainWindow, WidgetService widgets)
    {
        _mainWindow = mainWindow;
        _widgets = widgets;

        _icon = new TaskbarIcon
        {
            Icon = LoadIcon(),
            ToolTipText = "ASimpleCalendar",
            ContextMenu = BuildMenu()
        };
        _icon.TrayMouseDoubleClick += (_, _) => ShowMainWindow();
    }

    private static Icon LoadIcon()
    {
        var exePath = Environment.ProcessPath;
        if (exePath is not null)
        {
            var icon = Icon.ExtractAssociatedIcon(exePath);
            if (icon is not null)
            {
                return icon;
            }
        }

        return SystemIcons.Application;
    }

    public void ShowMainWindow()
    {
        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }

    public void Exit()
    {
        _icon.Dispose();
        App.ShutdownApplication();
    }

    private ContextMenu BuildMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(CreateItem("Открыть", ShowMainWindow));
        menu.Items.Add(new Separator());

        var clock = CreateItem("Часы", _widgets.ToggleClock);
        var calendar = CreateItem("Мини-календарь", _widgets.ToggleCalendar);
        var tasks = CreateItem("Задачи", _widgets.ToggleTasks);

        menu.Items.Add(clock);
        menu.Items.Add(calendar);
        menu.Items.Add(tasks);
        menu.Items.Add(new Separator());
        menu.Items.Add(CreateItem("Выход", Exit));

        return menu;
    }

    private static MenuItem CreateItem(string header, Action action)
    {
        var item = new MenuItem { Header = header };
        item.Click += (_, _) => action();
        return item;
    }
}
