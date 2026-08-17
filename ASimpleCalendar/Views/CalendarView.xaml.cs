using System.Windows;
using System.Windows.Controls;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Views;

public partial class CalendarView : UserControl
{
    private readonly CalendarViewModel _viewModel;

    public CalendarView()
    {
        InitializeComponent();
        _viewModel = new CalendarViewModel(App.Services.GetRequiredService<IEventRepository>());
        DataContext = _viewModel;
    }

    private void AddEvent_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new EventDialog(initialDate: _viewModel.SelectedDate ?? DateTime.Today)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            _viewModel.AddEvent(dialog.Result);
        }
    }

    private void EditEvent_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedEvent is not { } ev)
        {
            return;
        }

        var dialog = new EventDialog(ev)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            dialog.Result.Id = ev.Id;
            _viewModel.UpdateEvent(dialog.Result);
        }
    }

    private void DeleteEvent_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedEvent is not { } ev)
        {
            return;
        }

        var answer = MessageBox.Show(
            $"Удалить событие «{ev.Title}»?",
            "ASimpleCalendar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (answer == MessageBoxResult.Yes)
        {
            _viewModel.DeleteEvent(ev);
        }
    }
}
