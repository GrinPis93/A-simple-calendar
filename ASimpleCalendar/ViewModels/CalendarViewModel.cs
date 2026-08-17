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
    private CalendarViewMode _viewMode = CalendarViewMode.Month;

    [ObservableProperty]
    private DateTime _currentMonth;

    [ObservableProperty]
    private DateTime _weekStart;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private DateTime? _selectedDate;

    [ObservableProperty]
    private Event? _selectedEvent;

    [ObservableProperty]
    private bool _isMonthView = true;

    [ObservableProperty]
    private bool _isWeekView;

    [ObservableProperty]
    private bool _isDayView;

    [ObservableProperty]
    private bool _showSidePanel = true;

    public ObservableCollection<DayCellViewModel> Days { get; } = new();
    public ObservableCollection<DayColumnViewModel> WeekDays { get; } = new();
    public ObservableCollection<Event> SelectedDayEvents { get; } = new();

    public CalendarViewModel(IEventRepository events)
    {
        _events = events;
        var today = DateTime.Today;
        _currentMonth = new DateTime(today.Year, today.Month, 1);
        _weekStart = today.AddDays(-((int)today.DayOfWeek + 6) % 7);
        _selectedDate = today;
        Rebuild();
    }

    partial void OnViewModeChanged(CalendarViewMode value)
    {
        IsMonthView = value == CalendarViewMode.Month;
        IsWeekView = value == CalendarViewMode.Week;
        IsDayView = value == CalendarViewMode.Day;
        ShowSidePanel = value != CalendarViewMode.Day;
        Rebuild();
    }

    partial void OnCurrentMonthChanged(DateTime value)
    {
        if (ViewMode == CalendarViewMode.Month)
        {
            Rebuild();
        }
    }

    partial void OnWeekStartChanged(DateTime value)
    {
        if (ViewMode == CalendarViewMode.Week)
        {
            Rebuild();
        }
    }

    partial void OnSelectedDateChanged(DateTime? value)
    {
        if (value is not { } date)
        {
            return;
        }

        if (ViewMode == CalendarViewMode.Day)
        {
            Rebuild();
        }
        else
        {
            UpdateSelection();
            LoadSelectedDay(date);
        }
    }

    [RelayCommand]
    private void SetMonthView() => ViewMode = CalendarViewMode.Month;

    [RelayCommand]
    private void SetWeekView() => ViewMode = CalendarViewMode.Week;

    [RelayCommand]
    private void SetDayView() => ViewMode = CalendarViewMode.Day;

    [RelayCommand]
    private void Previous()
    {
        switch (ViewMode)
        {
            case CalendarViewMode.Month:
                CurrentMonth = CurrentMonth.AddMonths(-1);
                break;
            case CalendarViewMode.Week:
                WeekStart = WeekStart.AddDays(-7);
                break;
            case CalendarViewMode.Day:
                SelectedDate = (SelectedDate ?? DateTime.Today).AddDays(-1);
                break;
        }
    }

    [RelayCommand]
    private void Next()
    {
        switch (ViewMode)
        {
            case CalendarViewMode.Month:
                CurrentMonth = CurrentMonth.AddMonths(1);
                break;
            case CalendarViewMode.Week:
                WeekStart = WeekStart.AddDays(7);
                break;
            case CalendarViewMode.Day:
                SelectedDate = (SelectedDate ?? DateTime.Today).AddDays(1);
                break;
        }
    }

    [RelayCommand]
    private void GoToToday()
    {
        var today = DateTime.Today;
        CurrentMonth = new DateTime(today.Year, today.Month, 1);
        WeekStart = today.AddDays(-((int)today.DayOfWeek + 6) % 7);
        SelectedDate = today;
        Rebuild();
    }

    [RelayCommand]
    private void SelectDay(DayCellViewModel? cell)
    {
        if (cell is not null)
        {
            SelectedDate = cell.Date;
        }
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

        switch (ViewMode)
        {
            case CalendarViewMode.Month:
                RebuildMonth(ru);
                break;
            case CalendarViewMode.Week:
                RebuildWeek(ru);
                break;
            case CalendarViewMode.Day:
                RebuildDay(ru);
                break;
        }
    }

    private void RebuildMonth(CultureInfo ru)
    {
        var first = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        Title = ru.TextInfo.ToTitleCase(first.ToString("MMMM yyyy", ru));

        var offset = ((int)first.DayOfWeek + 6) % 7;
        var start = first.AddDays(-offset);
        var end = start.AddDays(42);

        var byDate = GroupByDate(start, end);
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

        if (!SelectedDate.HasValue)
        {
            SelectedDate = today;
        }

        LoadSelectedDay(SelectedDate.Value);
    }

    private void RebuildWeek(CultureInfo ru)
    {
        var start = WeekStart;
        var end = start.AddDays(7);

        Title = $"{start:dd MMMM} — {start.AddDays(6):dd MMMM yyyy}";

        var byDate = GroupByDate(start, end);
        var today = DateTime.Today;

        WeekDays.Clear();
        for (var i = 0; i < 7; i++)
        {
            var date = start.AddDays(i);
            byDate.TryGetValue(date, out var dayEvents);
            WeekDays.Add(new DayColumnViewModel
            {
                Date = date,
                DayTitle = ru.TextInfo.ToTitleCase(date.ToString("ddd, dd MMM", ru)),
                IsToday = date == today,
                Events = dayEvents ?? new List<Event>()
            });
        }

        if (!SelectedDate.HasValue || SelectedDate < start || SelectedDate >= end)
        {
            SelectedDate = start;
        }

        LoadSelectedDay(SelectedDate.Value);
    }

    private void RebuildDay(CultureInfo ru)
    {
        var date = SelectedDate ?? DateTime.Today;

        if (!SelectedDate.HasValue)
        {
            SelectedDate = date;
        }

        Title = ru.TextInfo.ToTitleCase(date.ToString("dddd, d MMMM yyyy", ru));
        LoadSelectedDay(date);
    }

    private Dictionary<DateTime, List<Event>> GroupByDate(DateTime start, DateTime end)
    {
        return GetEventsInRange(start, end)
            .GroupBy(e => e.StartDate.Date)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private List<Event> GetEventsInRange(DateTime start, DateTime end)
    {
        var result = new List<Event>(_events.GetByRange(start, end));

        foreach (var ev in _events.GetAll().Where(e => e.Repeat != RepeatRule.None))
        {
            result.AddRange(ExpandOccurrences(ev, start, end));
        }

        return result;
    }

    private static IEnumerable<Event> ExpandOccurrences(Event ev, DateTime start, DateTime end)
    {
        var limit = ev.RepeatUntil ?? end;
        if (limit > end)
        {
            limit = end;
        }

        var current = ev.StartDate;
        while (current < start)
        {
            var next = NextOccurrence(current, ev.Repeat);
            if (next <= current)
            {
                yield break;
            }

            current = next;
        }

        while (current <= limit)
        {
            yield return CloneForDate(ev, current);

            var next = NextOccurrence(current, ev.Repeat);
            if (next <= current)
            {
                yield break;
            }

            current = next;
        }
    }

    private static DateTime NextOccurrence(DateTime current, RepeatRule repeat) => repeat switch
    {
        RepeatRule.Daily => current.AddDays(1),
        RepeatRule.Weekly => current.AddDays(7),
        RepeatRule.Monthly => current.AddMonths(1),
        RepeatRule.Yearly => current.AddYears(1),
        _ => current
    };

    private static Event CloneForDate(Event ev, DateTime date)
    {
        var duration = ev.EndDate.HasValue ? ev.EndDate.Value - ev.StartDate : TimeSpan.Zero;

        return new Event
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            StartDate = date,
            EndDate = ev.EndDate.HasValue ? date + duration : null,
            AllDay = ev.AllDay,
            Category = ev.Category,
            Color = ev.Color,
            Repeat = ev.Repeat,
            RepeatUntil = ev.RepeatUntil,
            CreatedAt = ev.CreatedAt
        };
    }

    private void UpdateSelection()
    {
        foreach (var day in Days)
        {
            day.IsSelected = SelectedDate.HasValue && day.Date == SelectedDate.Value;
        }
    }

    private void LoadSelectedDay(DateTime date)
    {
        SelectedDayEvents.Clear();
        foreach (var ev in GetEventsInRange(date.Date, date.Date.AddDays(1)))
        {
            SelectedDayEvents.Add(ev);
        }
    }
}
