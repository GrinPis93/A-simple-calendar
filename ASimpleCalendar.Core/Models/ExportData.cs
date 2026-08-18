namespace ASimpleCalendar.Models;

public class ExportData
{
    public int Version { get; set; }
    public List<Event> Events { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
    public List<Reminder> Reminders { get; set; } = new();
    public Dictionary<string, string> Settings { get; set; } = new();
}
