using System;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Views.Base;
using System.IO;
using System.Threading.Tasks;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Utilities;
using Microsoft.Maui;
using System.Threading.Tasks;
using FairFlexxApps.Capture.ViewModels.Commons;
using FairFlexxApps.Capture.ViewModels;

namespace FairFlexxApps.Capture.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : BasePage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        #region Instance

        private static HomePage _instance;

        public static HomePage Instance => _instance ?? (_instance = new HomePage());

        #endregion

        #region OnAppearing

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetUpLogo();
        }

        #endregion

        #region Set up Logo

        private void SetUpLogo()
        {
            imageLogo.Source = "ic_loading.png";

            SetPositionLogo();
            SetLogoUser();

            imageLogo.WidthRequest = (App.ScreenWidth / 2); // App.DisplayScaleFactor;
            imageLogo.HeightRequest = (((App.ScreenHeight - 50) / 3) / 2); // App.DisplayScaleFactor;

            //lblList.FontSize = 19; // App.DisplayScaleFactor;

            //lblWithCard.FontSize = 19; // App.DisplayScaleFactor;

            //lblWithoutCard.FontSize = 19; // App.DisplayScaleFactor;
        }

        #endregion

        #region SetLogoUser

        private void SetLogoUser()
        {
            if (App.Settings.LogoUser != null)
            {
                var stream = new MemoryStream(App.Settings.LogoUser);
                imageLogo.Source = ImageSource.FromStream(() => stream);
            }
            else
            {
                imageLogo.Source = "splash_logo.png";
            }

            imageLogo.WidthRequest = (App.ScreenWidth / 3);
            imageLogo.HeightRequest = ((App.ScreenHeight - 50) / 3) / 2;
        }

        #endregion

        #region SetPositionLogo

        private void SetPositionLogo()
        {
            imageLogo.VerticalOptions = GetVerticalIcon();
            imageLogo.HorizontalOptions = GetHorizontalIcon();
        }

        #endregion

        #region GetHorizontalIcon

        private LayoutOptions GetHorizontalIcon()
        {
            LayoutOptions position = LayoutOptions.StartAndExpand;
            switch (App.Settings.HorizontalIcon)
            {
                case HorizontalPositions.Left:
                    position = LayoutOptions.StartAndExpand;
                    break;
                case HorizontalPositions.Middle:
                    position = LayoutOptions.CenterAndExpand;
                    break;
                case HorizontalPositions.Right:
                    position = LayoutOptions.EndAndExpand;
                    break;
            }

            return position;
        }

        #endregion

        #region GetVerticalIcon

        private LayoutOptions GetVerticalIcon()
        {
            LayoutOptions position = LayoutOptions.StartAndExpand;
            switch (App.Settings.VerticalIcon)
            {
                case VerticalPostions.Top:
                    position = LayoutOptions.StartAndExpand;
                    break;
                case VerticalPostions.Center:
                    position = LayoutOptions.CenterAndExpand;
                    break;
                case VerticalPostions.Bottom:
                    position = LayoutOptions.EndAndExpand;
                    break;
            }

            return position;
        }

        #endregion

        #region Check Device Orientation

        public static bool IsPortrait(Page p) { return p.Width < p.Height; }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called

            var deviceOrientation = DependencyService.Get<IDeviceOrientation>().GetOrientation();

            if (deviceOrientation != App.DeviceOrientations)
            {
                App.ScreenWidth = width;
                App.ScreenHeight = height;

                RefreshUI();
            }

            App.DeviceOrientations = deviceOrientation;
        }

        #endregion

        #region RefreshUI

        public void RefreshUI()
        {
            //InitializeComponent();

            SetUpLogo();
        }

        #endregion

        private async void AnimationSettingsOnTapped(object sender, System.EventArgs e)
        {
            SettingsPage.AnchorX = 0.48;
            SettingsPage.AnchorY = 0.48;
            await SettingsPage.ScaleTo(0.8, 50, Easing.Linear);
            var vm = (HomePageViewModel)BindingContext;
            vm?.SettingsExe();
            await SettingsPage.ScaleTo(1, 50, Easing.Linear);
           
        }
    }
}
