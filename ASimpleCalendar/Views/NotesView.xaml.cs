using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using ASimpleCalendar.Services;
using ASimpleCalendar.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ASimpleCalendar.Views;

public partial class NotesView : UserControl
{
    private readonly NotesViewModel _viewModel;

    public NotesView()
    {
        InitializeComponent();
        _viewModel = new NotesViewModel(App.Services.GetRequiredService<INoteRepository>());
        DataContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        RebuildContent();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NotesViewModel.SelectedNote))
        {
            RebuildContent();
        }
    }

    private void RebuildContent()
    {
        ContentText.Inlines.Clear();

        if (_viewModel.SelectedNote is { } note)
        {
            foreach (var inline in MarkdownConverter.ToInlines(note.Content))
            {
                ContentText.Inlines.Add(inline);
            }
        }
    }

    private void AddNote_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NoteDialog
        {
            Owner = Window.GetWindow(this)!
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            _viewModel.AddNote(dialog.Result);
        }
    }

    private void EditNote_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedNote is not { } note)
        {
            return;
        }

        var dialog = new NoteDialog(note)
        {
            Owner = Window.GetWindow(this)!
        };

        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            dialog.Result.Id = note.Id;
            dialog.Result.CreatedAt = note.CreatedAt;
            _viewModel.UpdateNote(dialog.Result);
        }
    }

    private void DeleteNote_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedNote is not { } note)
        {
            return;
        }

        var answer = MessageBox.Show(
            $"Удалить заметку «{note.Title}»?",
            "ASimpleCalendar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (answer == MessageBoxResult.Yes)
        {
            _viewModel.DeleteNote(note);
        }
    }

    private void TogglePin_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedNote is { } note)
        {
            _viewModel.TogglePin(note);
        }
    }

    private void NotesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedNote is not null)
        {
            EditNote_Click(sender, e);
        }
    }
}
