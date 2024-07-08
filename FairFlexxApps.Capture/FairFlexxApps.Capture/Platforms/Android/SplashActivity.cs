using Android.App;
using Android.OS;

namespace FairFlexxApps.Capture.Platforms.Android;

[Activity(Label = "SplashActivity")]
public class SplashActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Create your application here
    }

    [Activity(
        Theme = "@style/SplashTheme",
       Icon = "@mipmap/icon_app",
        RoundIcon = "@mipmap/icon_app",
        //RoundIcon = "@drawable/Icon",
        MainLauncher = true,
        NoHistory = true,
        //Exported = true,
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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            StartActivity(typeof(MainActivity));
        }
    }
}
