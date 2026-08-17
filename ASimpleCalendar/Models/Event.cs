namespace ASimpleCalendar.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AllDay { get; set; }
    public string? Category { get; set; }
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
}
