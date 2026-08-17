using System.Windows.Controls;
using ASimpleCalendar.Services;
using ASimpleCalendar.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel(App.Services.GetRequiredService<WidgetService>());
    }
}
