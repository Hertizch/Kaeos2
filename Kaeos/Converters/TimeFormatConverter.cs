﻿using System;
using System.Globalization;
using System.Windows.Data;
using Kaeos.Modules;

namespace Kaeos.Converters
{
    public class TimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "n/a";

            if (ControlModule.AppConfigModule.DateTimeModule_TimeFormat == null)
                return "n/a";

            var dateTime = (DateTime)value;
            return dateTime.ToString(ControlModule.AppConfigModule.DateTimeModule_TimeFormat.Code);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
