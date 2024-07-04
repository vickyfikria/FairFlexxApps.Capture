using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Models;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class HintEventListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null && ((ObservableCollection<EventModel>)value).Any();
            return TranslateExtension.Get(result ? "ChooseTheEventAndLanguage" : "NoEvent");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
