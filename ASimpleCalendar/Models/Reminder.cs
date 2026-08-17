namespace ASimpleCalendar.Models;

public class Reminder
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime RemindAt { get; set; }
    public RepeatRule Repeat { get; set; }
    public bool IsActive { get; set; }
    public int? EventId { get; set; }
    public DateTime? LastNotifiedAt { get; set; }
}
