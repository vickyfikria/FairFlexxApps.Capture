using System;
using System.Globalization;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public Color TrueValue { get; set; }

        public Color FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Color)value;
        }
    }
}
