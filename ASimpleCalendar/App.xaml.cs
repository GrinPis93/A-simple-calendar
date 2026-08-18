using System.Windows;
using System.Windows.Threading;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace ASimpleCalendar;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static bool IsShuttingDown { get; private set; }

    private MainWindow? _mainWindow;
    private TrayService? _tray;
    private HotKeyService? _hotKeys;

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
        services.AddSingleton<CategoryService>();
        services.AddSingleton<DataExportService>();
        Services = services.BuildServiceProvider();

        var database = Services.GetRequiredService<DatabaseService>();
        database.Initialize();

        if (!database.CheckIntegrity())
        {
            AppLogger.Log("База данных повреждена (PRAGMA quick_check != ok).");
        }

        Services.GetRequiredService<DataExportService>().EnsureDailyBackup();

        Services.GetRequiredService<ReminderScheduler>();

        var settings = Services.GetRequiredService<ISettingsRepository>();
        ThemeHelper.Apply(ThemeHelper.Parse(settings.Get("theme")));
        ThemeHelper.ApplyAccent(settings.Get("accent"));

        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;

        _mainWindow = new MainWindow();
        _mainWindow.Show();

        _tray = new TrayService(_mainWindow, Services.GetRequiredService<WidgetService>());

        _hotKeys = new HotKeyService();
        _hotKeys.Register(
            _mainWindow,
            onShow: () => _tray?.ShowMainWindow(),
            onNewNote: () =>
            {
                _tray?.ShowMainWindow();
                _mainWindow.ShowNotesAndCreate();
            },
            onNewReminder: () =>
            {
                _tray?.ShowMainWindow();
                _mainWindow.ShowRemindersAndCreate();
            });
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

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        var settings = Services.GetRequiredService<ISettingsRepository>();
        if (ThemeHelper.Parse(settings.Get("theme")) == ThemeMode.Auto)
        {
            ThemeHelper.Apply(ThemeMode.Auto);
            ThemeHelper.ApplyAccent(settings.Get("accent"));
        }
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
