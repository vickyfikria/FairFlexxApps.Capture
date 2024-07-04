using System;
using System.Globalization;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class ItemTappedEventArgsConverter : IValueConverter
    {
        #region Convert

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is ListView view)
                if (view.SelectedItem != null)
                    view.SelectedItem = null;

            var eventArgs = value as ItemTappedEventArgs;
            return eventArgs?.Item;
        }

        #endregion

        #region ConvertBack

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
