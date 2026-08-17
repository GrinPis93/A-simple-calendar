using System.Windows;
using ASimpleCalendar.Data;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace ASimpleCalendar;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<DatabaseService>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<INoteRepository, NoteRepository>();
        services.AddSingleton<IReminderRepository, ReminderRepository>();
        services.AddSingleton<ISettingsRepository, SettingsRepository>();
        Services = services.BuildServiceProvider();

        Services.GetRequiredService<DatabaseService>().Initialize();

        // Тёмная тема по умолчанию; в дальнейшем будет читаться из настроек.
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);

        _mainWindow = new MainWindow();
        _mainWindow.Show();
    }
}
