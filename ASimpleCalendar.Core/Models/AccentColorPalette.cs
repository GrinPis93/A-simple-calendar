namespace ASimpleCalendar.Models;

public static class AccentColorPalette
{
    public static readonly IReadOnlyList<AccentOption> Items = new List<AccentOption>
    {
        new("Системный", ""),
        new("Синий", "#4F6BED"),
        new("Зелёный", "#2FA96B"),
        new("Красный", "#D64545"),
        new("Оранжевый", "#E8802A"),
        new("Фиолетовый", "#9C4FD6"),
        new("Голубой", "#0EA5E9"),
        new("Жёлтый", "#EAB308"),
        new("Розовый", "#EC4899"),
        new("Бирюзовый", "#14B8A6"),
        new("Серый", "#64748B")
    };
}
