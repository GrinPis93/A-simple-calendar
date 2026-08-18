namespace ASimpleCalendar.Models;

public class CategoryItem
{
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#64748B";

    public override string ToString() => Name;
}
