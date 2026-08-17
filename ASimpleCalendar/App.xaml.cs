using System.Windows;
using Wpf.Ui.Appearance;

namespace ASimpleCalendar;

public partial class App : Application
{
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Тёмная тема по умолчанию; в дальнейшем будет читаться из настроек.
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);

        _mainWindow = new MainWindow();
        _mainWindow.Show();
    }
}
