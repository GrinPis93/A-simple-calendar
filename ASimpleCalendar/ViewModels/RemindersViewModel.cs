using System.Collections.ObjectModel;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASimpleCalendar.ViewModels;

public partial class RemindersViewModel : ObservableObject
{
    private readonly IReminderRepository _reminders;

    [ObservableProperty]
    private Reminder? _selectedReminder;

    public ObservableCollection<Reminder> Reminders { get; } = new();

    public RemindersViewModel(IReminderRepository reminders)
    {
        _reminders = reminders;
        Reload();
    }

    public void Reload()
    {
        var selectedId = SelectedReminder?.Id;
        Reminders.Clear();
        foreach (var reminder in _reminders.GetAll())
        {
            Reminders.Add(reminder);
        }

        SelectedReminder = selectedId is null ? Reminders.FirstOrDefault() : Reminders.FirstOrDefault(r => r.Id == selectedId);
    }

    public void Add(Reminder item)
    {
        _reminders.Add(item);
        Reload();
    }

    public void Update(Reminder item)
    {
        _reminders.Update(item);
        Reload();
    }

    public void Delete(Reminder item)
    {
        _reminders.Delete(item.Id);
        SelectedReminder = null;
        Reload();
    }

    public void ToggleActive(Reminder item)
    {
        item.IsActive = !item.IsActive;
        _reminders.Update(item);
        Reload();
    }
}
