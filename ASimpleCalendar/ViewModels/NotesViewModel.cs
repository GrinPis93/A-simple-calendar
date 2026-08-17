using System.Collections.ObjectModel;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASimpleCalendar.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private const string AllCategory = "Все";

    private readonly INoteRepository _notes;
    private List<Note> _all = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = AllCategory;

    [ObservableProperty]
    private Note? _selectedNote;

    public ObservableCollection<Note> Notes { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();

    public NotesViewModel(INoteRepository notes)
    {
        _notes = notes;
        Reload();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();

    public void AddNote(Note item)
    {
        _notes.Add(item);
        Reload();
    }

    public void UpdateNote(Note item)
    {
        _notes.Update(item);
        Reload();
    }

    public void DeleteNote(Note item)
    {
        _notes.Delete(item.Id);
        SelectedNote = null;
        Reload();
    }

    public void TogglePin(Note item)
    {
        item.IsPinned = !item.IsPinned;
        item.UpdatedAt = DateTime.Now;
        _notes.Update(item);
        Reload();
    }

    public void Reload()
    {
        _all = _notes.GetAll();

        var current = SelectedCategory;
        Categories.Clear();
        Categories.Add(AllCategory);
        foreach (var category in _all
                     .Select(n => n.Category)
                     .Where(c => !string.IsNullOrWhiteSpace(c))
                     .Distinct()
                     .OrderBy(c => c))
        {
            Categories.Add(category!);
        }

        if (!Categories.Contains(current))
        {
            current = AllCategory;
        }

        SelectedCategory = current;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var selectedId = SelectedNote?.Id;
        Notes.Clear();

        IEnumerable<Note> filtered = _all;

        if (SelectedCategory != AllCategory)
        {
            filtered = filtered.Where(n => n.Category == SelectedCategory);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(n =>
                n.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                n.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (n.Tags ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var note in filtered)
        {
            Notes.Add(note);
        }

        SelectedNote = selectedId is null ? Notes.FirstOrDefault() : Notes.FirstOrDefault(n => n.Id == selectedId);
    }
}
