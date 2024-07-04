using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class StringErrorConverter : IValueConverter
    {
        /// <summary>
        /// Reverse the result: If any result --> return false. Else return true
        /// </summary>
        public string StringError { get; set; } ="";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value.ToString().StringToBool() == true)
                return StringError = TranslateExtension.Get("UpdateAppError");
            else
                return StringError = TranslateExtension.Get("MissingConfigError");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
