using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Models.LeadModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class NewCardTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ObservableCollection<LeadTypeModel>)value).Count == 0 
                ? TranslateExtension.Get("NewCard") 
                : TranslateExtension.Get("FurtherCard");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
