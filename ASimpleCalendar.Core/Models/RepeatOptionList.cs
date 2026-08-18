namespace ASimpleCalendar.Models;

public static class RepeatOptionList
{
    public static readonly RepeatOption[] All =
    {
        new("Без повтора", RepeatRule.None),
        new("Ежедневно", RepeatRule.Daily),
        new("Еженедельно", RepeatRule.Weekly),
        new("Ежемесячно", RepeatRule.Monthly),
        new("Ежегодно", RepeatRule.Yearly)
    };
}
