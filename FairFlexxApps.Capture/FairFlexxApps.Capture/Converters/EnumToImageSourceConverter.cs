using FairFlexxApps.Capture.Enums;
using System;
using System.Globalization;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class EnumToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var statusOfLeadModel = (StatusOfLeadModel)value;

            switch (statusOfLeadModel)
            {
                case StatusOfLeadModel.EmptyCircle:
                    return ImageSource.FromFile("ic_none.png");
                case StatusOfLeadModel.HalfCircle:
                    return ImageSource.FromFile("ic_half.png");
                case StatusOfLeadModel.CheckedCircle:
                    return ImageSource.FromFile("ic_full.png");
                case StatusOfLeadModel.QuestionMark:
                    return ImageSource.FromFile("ic_warning.png");
            }

            return ImageSource.FromFile("ic_none.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value;
            //return $"ic_{((string)value)?.ToLower()}.png";
        }
    }
}
