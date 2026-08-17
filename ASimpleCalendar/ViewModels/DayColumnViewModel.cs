using ASimpleCalendar.Models;

namespace ASimpleCalendar.ViewModels;

public class DayColumnViewModel
{
    public DateTime Date { get; init; }
    public string DayTitle { get; init; } = string.Empty;
    public bool IsToday { get; init; }
    public List<Event> Events { get; init; } = new();
}
