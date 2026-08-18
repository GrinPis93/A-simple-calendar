namespace ASimpleCalendar.Models;

public static class CategoryPalette
{
    public static readonly IReadOnlyList<CategoryItem> Items = new List<CategoryItem>
    {
        new() { Name = "Работа", Color = "#4F6BED" },
        new() { Name = "Личное", Color = "#E8802A" },
        new() { Name = "Здоровье", Color = "#2FA96B" },
        new() { Name = "Учёба", Color = "#9C4FD6" },
        new() { Name = "Встреча", Color = "#D64545" },
        new() { Name = "Другое", Color = "#64748B" }
    };
}
