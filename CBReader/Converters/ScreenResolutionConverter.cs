using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CBReader.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class ScreenResolutionConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Used in MainWindow.xaml. Used to always fit the users' screen, no matter the resolution. The resolution is scalled accordingly with the Parameter value, e.g. 1920 x 0.9 = 1728, 1080 * 0,9 = 972.
    /// The way it's implemented in the main window is that the program takes users' current screen resolution and converts it by x and y.
    /// @ see more at: https://stackoverflow.com/questions/8121906/resize-wpf-window-and-contents-depening-on-screen-resolution    
    /// </summary>


    private static ScreenResolutionConverter? _instance;
    public ScreenResolutionConverter() { }      // parameterless constructor. It can be deleted, though
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
        return size.ToString("G0", CultureInfo.InvariantCulture);       // "G0" formats string properly with the invariant culture
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _instance ?? (_instance = new ScreenResolutionConverter());      // if exists, return it. If it doesnt, create a new one and store in _instance, then return.
    }
}
