using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Maui;

namespace FairFlexxApps.Capture.Views.ViewCells
{
    public class SideMenuViewCell : ViewCell
    {
        public SideMenuViewCell()
        {
            StackLayout cellWrapper = new StackLayout()
            {
                Margin = new Thickness(0),
                Orientation = StackOrientation.Horizontal,
                //Margin = new Thickness(5,0,0,0),
                //BackgroundColor = Color.White,
            };

            var imageItem = new Image()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            imageItem.SetBinding(Image.SourceProperty, new Binding("ImageSource", BindingMode.TwoWay));

            var menuItem = new Label()
            {
                FontSize = 16,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            menuItem.SetBinding(Label.TextProperty, new Binding("SideMenuID"));

            //add views to the view hierarchy
            cellWrapper.Children.Add(imageItem);
            cellWrapper.Children.Add(menuItem);

            View = cellWrapper;
        }
    }
}
