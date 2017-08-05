using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Kaeos.Converters
{
    public class ValueGreaterThanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorBrush = new SolidColorBrush(Colors.White);

            if ((float?)value >= 70)
                colorBrush = new SolidColorBrush(Color.FromRgb(240, 79, 79));

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
