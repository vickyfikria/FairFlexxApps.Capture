using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.Utilities;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class StreamConverter : IValueConverter
    {
        #region Convert

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            var result = (ObservableCollection<ScannerResult>)value;

            var stream = new MemoryStream(result.ElementAt(0).ByteImage);
            var imageSource = ImageSource.FromStream(() => stream);

            return imageSource;
        }

        #endregion

        #region ConvertBack

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

    public class ImageFilterConverter : IValueConverter
    {
        #region Convert

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null) return ;
            //var imageLink = (value as DocumentScannerResult);

            //return imageLink?.Pages[0].AvailablePreview;

            return "ic_color.png";
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
