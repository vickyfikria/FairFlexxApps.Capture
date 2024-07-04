using System;
using System.Globalization;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
	public class NotConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Equals (value, false);
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Equals (value, false);
		}
	}
}
