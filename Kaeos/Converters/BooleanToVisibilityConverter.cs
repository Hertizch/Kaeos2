using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kaeos.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var objValue = value != null && (bool)value;

            if (!objValue)
                return Visibility.Collapsed;

            if (!objValue && (string)parameter == "invert")
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
