using FairFlexxApps.Capture.Constants;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Services;
using FairFlexxApps.Capture.Services.LocalDatabase;
using FairFlexxApps.Capture.Services.Settings;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace FairFlexxApps.Capture
{
    public partial class App : Application
    {

        public App()
        {
            InitAppCenter();
            InitDatabase();

            StartApp();
        }

        #region Properties 

        public new static App Current => Application.Current as App;
        public static double ScreenWidth;
        public static double ScreenHeight;

        public static double DisplayScreenHeight { get; set; }
        public static double DisplayScreenWidth { get; set; }
        public static double DisplayScaleFactor { get; set; }

        public static double RatioFontSizeResponsiveLayout { get; set; }

        public static bool IsBusy = true;

        public static DeviceOrientations DeviceOrientations { get; set; } = DeviceOrientations.Undefined; //DependencyService.Get<IDeviceOrientation>().GetOrientation();

        //public static bool IsUseFormatXml = true;

        private ISqLiteService _sqLiteService;
        public static AppSettings Settings { get; set; }
        public static AppConfig Configs { get; set; }

        #endregion

        #region InitAppCenter

        private void InitAppCenter()
        {
            AppCenter.Start($"android={SdkKeyConstants.AppCenterAndroid};" +
                            $"ios={SdkKeyConstants.AppCenteriOS}",
                typeof(Analytics), typeof(Crashes));
        }

        #endregion

        #region InitDatabase

        private void InitDatabase()
        {
            var connectionService = DependencyService.Get<IDatabaseConnection>();
            _sqLiteService = new SqLiteService(connectionService);
        }

        #endregion

        #region StartApp

        private void StartApp()
        {
            Settings = new AppSettings();
            Configs = new AppConfig();
            Settings = _sqLiteService.GetSettings();
            Configs = _sqLiteService.GetConfig();
            ConfigUI();

            // Please remove in next version (v1.4.11 - 63)
            if (Settings.LastError == null)
            {
                Settings.LastError = string.Empty;
                _sqLiteService.Insert(Settings);
            }

            // Move to PrismStartup
            //if (Settings.IsActivated)
            //    await NavigationService.NavigateAsync(
            //        new Uri($"{PageManager.NavigationHomeUri}/{PageManager.NavigationPage}/{PageManager.HomePage}"));
            //else
            //    await NavigationService.NavigateAsync($"{PageManager.ActivationCodePage}");
        }

        #endregion

        #region ConfigUI

        public static void ConfigUI()
        {
            //RatioFontSizeResponsiveLayout = (Math.Max(App.DisplayScreenWidth, App.DisplayScreenHeight) > 1620) ? 4 : 0;
            int widthButton = (Math.Max(App.DisplayScreenWidth, App.DisplayScreenHeight) > 1620) ? 25 : 0;

            //App.Current.Resources["logoPlus"] = 95 / App.DisplayScaleFactor;
            //App.Current.Resources["labelBelowLogo"] = (20 + x) / App.DisplayScaleFactor;
            App.Current.Resources["HugeLabelFont"] = 22 + (int)Settings.FontSize; // App.DisplayScaleFactor;
            App.Current.Resources["SuperLargeLabelFont"] = 20 + (int)Settings.FontSize; // App.DisplayScaleFactor;
            App.Current.Resources["LargeLabelFont"] = 18 + (int)Settings.FontSize; // App.DisplayScaleFactor;
            App.Current.Resources["NormalLabelFont"] = (16 + (int)Settings.FontSize); // App.DisplayScaleFactor;
            App.Current.Resources["SmallLabelFont"] = (14 + (int)Settings.FontSize); // App.DisplayScaleFactor;

            App.Current.Resources["WidthButtonTopView"] = 170 + widthButton + Settings.FontSize;
            App.Current.Resources["LogoPlus"] = 100 + widthButton; // App.DisplayScaleFactor;

            //App.Current.Resources["heightTopContentView"] = 50 / App.DisplayScaleFactor;
            //App.Current.Resources["fontSizeTopContentView"] = (15 + x) / App.DisplayScaleFactor;
            //App.Current.Resources["WidthFrameStyleV1"] = 160 / App.DisplayScaleFactor;

            //App.Current.Resources["imageStatusLogo"] = 30 / App.DisplayScaleFactor;

            //// Button Edit
            //App.Current.Resources["WidthEditButton"] = 100 / App.DisplayScaleFactor;
            //App.Current.Resources["HeightEditButton"] = 50 / App.DisplayScaleFactor;

            //App.Current.Resources["PageTitlePadding"] = 10 / App.DisplayScaleFactor;


        }

        #endregion
    }
}

