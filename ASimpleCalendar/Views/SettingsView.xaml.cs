using System.Windows;
using System.Windows.Controls;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using ASimpleCalendar.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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
}
