using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginContentView : ContentView
    {
        public LoginContentView()
        {
            InitializeComponent();
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                FrameLogin.WidthRequest = (App.ScreenWidth / 1.75);// / App.DisplayScaleFactor;
                ConfigUI();
            }
        }

        private void ConfigUI()
        {
            double marginTemp = 50 / App.DisplayScaleFactor;
            int x = (Math.Max(App.ScreenHeight, App.ScreenWidth) > 1320) ? 2 : 0;
            stack.Padding = new Thickness(marginTemp, 25 / App.DisplayScaleFactor, marginTemp, 40 / App.DisplayScaleFactor);
            uploadContent.Margin = new Thickness(marginTemp, 0);
            titleLogin.Margin = new Thickness(marginTemp, 0);
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            entPassword.IsPassword = !entPassword.IsPassword;
        }
        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    if (width != App.ScreenWidth || height != App.ScreenHeight)
        //    {
        //        App.ScreenWidth = width;
        //        App.ScreenHeight = height;
        //        if (width > height)
        //        {
        //            double marginTemp = 50 / App.DisplayScaleFactor;
        //            InitializeComponent();
        //            FrameLogin.WidthRequest = (App.ScreenWidth / 1.75);
        //            stack.Padding = new Thickness(marginTemp, 25 / 2, marginTemp, 40 / 2);
        //        }
        //    }
        //}
    }
}
