using System.Windows;
using System.Windows.Threading;
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
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        try
        {
            base.OnStartup(e);
            Initialize();
        }
        catch (Exception ex)
        {
            AppLogger.Log("Ошибка запуска приложения", ex);
            ShowFatalError(ex);
            ShutdownApplication();
        }
    }

    private void Initialize()
    {
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

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        AppLogger.Log("Необработанное исключение в UI-потоке", e.Exception);
        e.Handled = true;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        AppLogger.Log("Необработанное исключение", e.ExceptionObject as Exception);
    }

    private static void ShowFatalError(Exception ex)
    {
        try
        {
            MessageBox.Show(
                "Не удалось запустить ASimpleCalendar.\n\n" + ex.Message +
                "\n\nПодробности записаны в файл:\n" + AppLogger.LogPath,
                "ASimpleCalendar",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch
        {
            // если не удалось показать окно — лог уже записан
        }
    }

    public static void ShutdownApplication()
    {
        IsShuttingDown = true;
        Current.Shutdown();
    }
}
