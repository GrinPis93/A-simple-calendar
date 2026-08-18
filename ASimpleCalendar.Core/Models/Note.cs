namespace ASimpleCalendar.Models;

public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
