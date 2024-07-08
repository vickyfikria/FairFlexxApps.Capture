using FairFlexxApps.Capture.Views;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture;
using FairFlexxApps.Capture.Droid.Utilities;  //.Platforms.Android.Utilities;

namespace FairFlexxApps.Capture;

internal static class PrismStartup
{

    public static void Configure(PrismAppBuilder builder)
    {
        builder.RegisterTypes(RegisterTypes).OnAppStart(async (container,navigation) =>
            {
                if (App.Settings.IsActivated)
                    await navigation.NavigateAsync(
                        new Uri($"{PageManager.NavigationHomeUri}/{PageManager.NavigationPage}/{PageManager.HomePage}"));
                else
                    await navigation.NavigateAsync($"{PageManager.ActivationCodePage}");
            }
        );
    }

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<MainPage>()
                     .RegisterInstance(SemanticScreenReader.Default);

        containerRegistry.RegisterSingleton<IFileService, FileService >();


    }
}
