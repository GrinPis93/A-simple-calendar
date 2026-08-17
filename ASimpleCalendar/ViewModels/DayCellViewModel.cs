using ASimpleCalendar.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASimpleCalendar.ViewModels;

public partial class DayCellViewModel : ObservableObject
{
    public DateTime Date { get; init; }
    public int DayNumber => Date.Day;
    public bool IsCurrentMonth { get; init; }
    public bool IsToday { get; init; }
    public List<Event> Events { get; init; } = new();

    public IEnumerable<Event> VisibleEvents => Events.Take(3);
    public bool HasMore => Events.Count > 3;
    public string MoreText => $"+{Events.Count - 3}";

    [ObservableProperty]
    private bool _isSelected;
}
