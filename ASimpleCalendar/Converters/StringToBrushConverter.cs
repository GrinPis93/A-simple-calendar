using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ASimpleCalendar.Converters;

public class StringToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
            try
            {
                var brush = (Brush)new BrushConverter().ConvertFromString(s);
                if (brush is not null)
                {
                    return brush;
                }
            }
            catch
            {
                // fall through to default color
            }
        }

        return new SolidColorBrush(Color.FromRgb(100, 116, 139));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
