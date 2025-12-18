using CBReader.Model;
using System.Globalization;
using System.Windows.Data;

namespace CBReader.Converters;

public class StatusEnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Status comicStatus)
        {
            return comicStatus switch
            {
                Status.NotStarted => "Not started",
                Status.InProgress => "In progress",
                Status.Finished => "Finished",
                _ => "N/A"
            };
        }

        return "N/A";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
