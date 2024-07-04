using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class ImageByteResourceConverter : IValueConverter
    {
        #region Convert

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            var byteImage = (byte[])value;
            var stream = new MemoryStream(byteImage);
            var imageSource = ImageSource.FromStream(() => stream);

            return imageSource;
        }

        #endregion

        #region ConvertBack

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                // Do your translation lookup here, using whatever method you require
                var byteImage = (byte[])value;
                var stream = new MemoryStream(byteImage);
                var imageSource = ImageSource.FromStream(() => stream);

                return imageSource;
            }
            catch
            {
                return value;
            }
            
        }

        #endregion
    }
}
