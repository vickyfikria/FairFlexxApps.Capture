using System;
using System.Globalization;
using FairFlexxApps.Capture.Enums;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Converters
{
    public class EnumStatusMenuItemToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var statusOfSideMenu = (SideMenuItemStatus)value;

            switch (statusOfSideMenu)
            {
                case SideMenuItemStatus.None:
                    return ImageSource.FromFile("ic_none.png");
                case SideMenuItemStatus.Half:
                    return ImageSource.FromFile("ic_half.png");
                case SideMenuItemStatus.Full:
                    return ImageSource.FromFile("ic_full.png");
                case SideMenuItemStatus.Warning:
                    return ImageSource.FromFile("ic_warning.png");
            }
            return ImageSource.FromFile("ic_none.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ImageSource.FromFile("ic_none.png");
        }
    }
}
