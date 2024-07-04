using FairFlexxApps.Capture.Interfaces;
using FFImageLoading;
using FFImageLoading.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Mopups.Hosting;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;
using FairFlexxApps.Capture.Droid.Utilities;

namespace FairFlexxApps.Capture
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
            => MauiApp
                .CreateBuilder()
                .UsePrismApp<App>(PrismStartup.Configure)
                .UseFFImageLoading()
                .UseSkiaSharp()
                .ConfigureFonts(
                    fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                        //fonts.AddFont("Font_Awesome_5_Free-Regular-400.otf", "FontAwesome-Regular");
                        //fonts.AddFont("Font_Awesome_5_Free-Solid-900.otf", "FontAwesome-Solid");
                        //fonts.AddFont("Montserrat-Bold.ttf", "Montserrat-Bold");
                        //fonts.AddFont("Montserrat-Regular.ttf", "Montserrat-Regular");
                        //fonts.AddFont("SourceSansPro-Regular.ttf", "SourceSansPro-Regular");
                        //fonts.AddFont("SourceSansPro-Solid.ttf", "SourceSansPro-Solid");
                    })
            
                .ConfigureMopups()
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews()
                .Build();

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.RegisterServices<IFileService>(new FileService());

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            return mauiAppBuilder;

        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            return mauiAppBuilder;

        }
    }
}
