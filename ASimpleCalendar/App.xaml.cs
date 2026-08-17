using System.Windows;
using ASimpleCalendar.Data;
using ASimpleCalendar.Services;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace ASimpleCalendar;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static bool IsShuttingDown { get; private set; }

    private MainWindow? _mainWindow;
    private TrayService? _tray;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<DatabaseService>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<INoteRepository, NoteRepository>();
        services.AddSingleton<IReminderRepository, ReminderRepository>();
        services.AddSingleton<ISettingsRepository, SettingsRepository>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<ReminderScheduler>();
        services.AddSingleton<WidgetService>();
        services.AddSingleton<AutoStartService>();
        Services = services.BuildServiceProvider();

        Services.GetRequiredService<DatabaseService>().Initialize();
        Services.GetRequiredService<ReminderScheduler>();

        var settings = Services.GetRequiredService<ISettingsRepository>();
        var theme = settings.Get("theme");
        ApplicationThemeManager.Apply(theme == "light" ? ApplicationTheme.Light : ApplicationTheme.Dark);

        _mainWindow = new MainWindow();
        _mainWindow.Show();

        _tray = new TrayService(_mainWindow, Services.GetRequiredService<WidgetService>());
    }

    public static void ShutdownApplication()
    {
        IsShuttingDown = true;
        Current.Shutdown();
    }
}
