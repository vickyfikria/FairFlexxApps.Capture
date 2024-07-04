using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.Net;

namespace FairFlexxApps.Capture;
[Activity(Label = "FairFlexx Capture",
    Icon = "@mipmap/icon_app",
    MainLauncher = false,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
    ScreenOrientation = ScreenOrientation.User, WindowSoftInputMode = SoftInput.AdjustResize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        //TabLayoutResource = Resource.Layout.Tabbar;
        //ToolbarResource = Resource.Layout.Toolbar;
        base.OnCreate(bundle);

        Init(bundle);

        ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

    }



    #region Init

    public void Init(Bundle bundle)
    {
        //RequestPermission();
        //Xamarin.Forms.Forms.SetFlags(new string[] { "CarouselView_Experimental", "SwipeView_Experimental", "IndicatorView_Experimental" });
        //CarouselViewRenderer.Init();
        //CachedImageRenderer.Init(true);
        //Rg.Plugins.Popup.Popup.Init(this, bundle);
        //GoogleVisionBarCodeScanner.Droid.RendererInitializer.Init();

        InitScreen();
    }

    #endregion

    #region InitScreen

    private void InitScreen()
    {
        var width = Resources.DisplayMetrics.WidthPixels;
        var height = Resources.DisplayMetrics.HeightPixels;
        var density = Resources.DisplayMetrics.Density;

        App.ScreenWidth = (width - 0.5f) / density;
        App.ScreenHeight = (height - 0.5f) / density;

        App.DisplayScreenWidth = (double)Resources.DisplayMetrics.WidthPixels;
        App.DisplayScreenHeight = (double)Resources.DisplayMetrics.HeightPixels;
        App.DisplayScaleFactor = (double)Resources.DisplayMetrics.Density;
    }

    #endregion
}


}
