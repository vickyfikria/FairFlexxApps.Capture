using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using FairFlexxApps.Capture.Models.LeadModels;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class AnyScannerResultItemsToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Reverse the result: If any result --> return false. Else return true
        /// </summary>
        public bool IsReverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ObservableCollection<LeadTypeModel>)value).Any() ? IsReverse : !IsReverse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
