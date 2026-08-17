using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Views;

public partial class RemindersView : UserControl
{
    private readonly RemindersViewModel _viewModel;

    public RemindersView()
    {
        InitializeComponent();
        _viewModel = new RemindersViewModel(App.Services.GetRequiredService<IReminderRepository>());
        DataContext = _viewModel;
    }

    private void AddReminder_Click(object sender, RoutedEventArgs e)
    {
        CreateReminder();
    }

    public void CreateReminder()
    {
        var dialog = new ReminderDialog
        {
            Owner = Window.GetWindow(this)!
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            _viewModel.Add(dialog.Result);
        }
    }

    private void EditReminder_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedReminder is not { } reminder)
        {
            return;
        }

        var dialog = new ReminderDialog(reminder)
        {
            Owner = Window.GetWindow(this)!
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            dialog.Result.Id = reminder.Id;
            dialog.Result.LastNotifiedAt = reminder.LastNotifiedAt;
            _viewModel.Update(dialog.Result);
        }
    }

    private void DeleteReminder_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedReminder is not { } reminder)
        {
            return;
        }

        var answer = MessageBox.Show(
            $"Удалить напоминание «{reminder.Title}»?",
            "ASimpleCalendar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (answer == MessageBoxResult.Yes)
        {
            _viewModel.Delete(reminder);
        }
    }

    private void ToggleActive_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedReminder is { } reminder)
        {
            _viewModel.ToggleActive(reminder);
        }
    }

    private void RemindersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedReminder is not null)
        {
            EditReminder_Click(sender, e);
        }
    }
}
