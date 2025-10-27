using System.Globalization;
using System.Windows.Data;

namespace CBReader.Converters;

public class FavouriteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isFavourite = (bool)value;
        return isFavourite ? "Remove from favourites" : "Add to favourites";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
