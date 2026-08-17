using System.Windows;
using System.Windows.Controls;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using ASimpleCalendar.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace ASimpleCalendar.Views;

public partial class SettingsView : UserControl
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView()
    {
        InitializeComponent();
        _viewModel = new SettingsViewModel(
            App.Services.GetRequiredService<WidgetService>(),
            App.Services.GetRequiredService<ISettingsRepository>(),
            App.Services.GetRequiredService<AutoStartService>(),
            App.Services.GetRequiredService<CategoryService>());
        DataContext = _viewModel;
    }

    private void AddCategory_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CategoryDialog
        {
            Owner = Window.GetWindow(this)!
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            _viewModel.AddCategory(dialog.Result);
        }
    }

    private void RemoveCategory_Click(object sender, RoutedEventArgs e)
    {
        if (CategoryList.SelectedItem is CategoryItem item)
        {
            _viewModel.RemoveCategory(item);
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            FileName = "ASimpleCalendar-backup.json"
        };

        if (dialog.ShowDialog() == true)
        {
            App.Services.GetRequiredService<DataExportService>().ExportToFile(dialog.FileName);
            MessageBox.Show("Данные экспортированы.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(
            "Импорт заменит текущие данные. Продолжить?",
            "ASimpleCalendar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json"
        };

        if (dialog.ShowDialog() == true)
        {
            App.Services.GetRequiredService<DataExportService>().ImportFromFile(dialog.FileName);
            MessageBox.Show("Импорт завершён. Приложение будет перезапущено.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Information);
            Restart();
        }
    }

    private void Backup_Click(object sender, RoutedEventArgs e)
    {
        var path = App.Services.GetRequiredService<DataExportService>().CreateBackup();
        MessageBox.Show("Резервная копия сохранена:\n" + path, "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static void Restart()
    {
        var exe = Environment.ProcessPath;
        if (exe is not null)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(exe) { UseShellExecute = true });
        }

        App.ShutdownApplication();
    }
}
