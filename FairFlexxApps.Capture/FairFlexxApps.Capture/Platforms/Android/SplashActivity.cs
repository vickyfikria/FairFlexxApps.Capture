using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


namespace FairFlexxApps.Capture.Platforms.Android;

[Activity(
        Theme = "@style/SplashTheme",
        Icon = "@mipmap/icon_app",
        RoundIcon = "@mipmap/icon_app",

        MainLauncher = true,
        NoHistory = true,

        ScreenOrientation = ScreenOrientation.Portrait)]
[IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "http",
        DataHost = "www.Fairflexx.com")]

[IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataHost = "www.Fairflexx.com")]
public class SplashActivity : Activity
{


    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Create your application here
        StartActivity(typeof(MainActivity));
    }
}
