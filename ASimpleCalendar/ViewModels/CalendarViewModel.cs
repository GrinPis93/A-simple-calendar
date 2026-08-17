using System.Collections.ObjectModel;
using System.Globalization;
using ASimpleCalendar.Data;
using ASimpleCalendar.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASimpleCalendar.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly IEventRepository _events;

    [ObservableProperty]
    private DateTime _currentMonth;

    [ObservableProperty]
    private string _monthTitle = string.Empty;

    [ObservableProperty]
    private DateTime? _selectedDate;

    [ObservableProperty]
    private Event? _selectedEvent;

    public ObservableCollection<DayCellViewModel> Days { get; } = new();
    public ObservableCollection<Event> SelectedDayEvents { get; } = new();

    public CalendarViewModel(IEventRepository events)
    {
        _events = events;
        var today = DateTime.Today;
        _currentMonth = new DateTime(today.Year, today.Month, 1);
        Rebuild();
    }

    partial void OnCurrentMonthChanged(DateTime value) => Rebuild();

    [RelayCommand]
    private void PreviousMonth() => CurrentMonth = CurrentMonth.AddMonths(-1);

    [RelayCommand]
    private void NextMonth() => CurrentMonth = CurrentMonth.AddMonths(1);

    [RelayCommand]
    private void GoToToday()
    {
        var today = DateTime.Today;
        CurrentMonth = new DateTime(today.Year, today.Month, 1);
        SelectDay(Days.FirstOrDefault(d => d.Date == today));
    }

    [RelayCommand]
    private void SelectDay(DayCellViewModel? cell)
    {
        if (cell is null)
        {
            return;
        }

        SelectedDate = cell.Date;
        foreach (var day in Days)
        {
            day.IsSelected = day.Date == cell.Date;
        }

        LoadSelectedDay(cell.Date);
    }

    public void AddEvent(Event item)
    {
        _events.Add(item);
        Rebuild();
    }

    public void UpdateEvent(Event item)
    {
        _events.Update(item);
        Rebuild();
    }

    public void DeleteEvent(Event item)
    {
        _events.Delete(item.Id);
        SelectedEvent = null;
        Rebuild();
    }

    private void Rebuild()
    {
        var ru = CultureInfo.GetCultureInfo("ru-RU");
        var first = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        MonthTitle = ru.TextInfo.ToTitleCase(first.ToString("MMMM yyyy", ru));

        var offset = ((int)first.DayOfWeek + 6) % 7;
        var start = first.AddDays(-offset);
        var end = start.AddDays(42);

        var byDate = _events.GetByRange(start, end)
            .GroupBy(e => e.StartDate.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        var today = DateTime.Today;
        var selected = SelectedDate ?? today;

        Days.Clear();
        for (var i = 0; i < 42; i++)
        {
            var date = start.AddDays(i);
            byDate.TryGetValue(date, out var dayEvents);
            Days.Add(new DayCellViewModel
            {
                Date = date,
                IsCurrentMonth = date.Year == CurrentMonth.Year && date.Month == CurrentMonth.Month,
                IsToday = date == today,
                IsSelected = date == selected,
                Events = dayEvents ?? new List<Event>()
            });
        }

        if (SelectedDate.HasValue)
        {
            LoadSelectedDay(SelectedDate.Value);
        }
        else
        {
            SelectedDate = today;
            LoadSelectedDay(today);
        }
    }

    private void LoadSelectedDay(DateTime date)
    {
        SelectedDayEvents.Clear();
        foreach (var ev in _events.GetByRange(date.Date, date.Date.AddDays(1)))
        {
            SelectedDayEvents.Add(ev);
        }
    }
}
