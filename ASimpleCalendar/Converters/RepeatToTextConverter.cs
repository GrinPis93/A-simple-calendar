using System.Globalization;
using System.Windows.Data;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Converters;

public class RepeatToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        RepeatRule.Daily => "Ежедневно",
        RepeatRule.Weekly => "Еженедельно",
        RepeatRule.Monthly => "Ежемесячно",
        RepeatRule.Yearly => "Ежегодно",
        _ => "Без повтора"
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
