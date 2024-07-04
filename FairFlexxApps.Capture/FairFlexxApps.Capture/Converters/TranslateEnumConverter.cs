using FairFlexxApps.Capture.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class TranslateEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TranslateExtension.Get(string.IsNullOrEmpty((string)value) ? string.Empty : (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TranslateExtension.Get((string)value);
        }
    }
}
