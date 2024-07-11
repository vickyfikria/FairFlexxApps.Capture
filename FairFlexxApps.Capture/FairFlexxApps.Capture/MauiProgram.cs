using FairFlexxApps.Capture.Interfaces;
using FFImageLoading;
using FFImageLoading.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Mopups.Hosting;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;

using FairFlexxApps.Capture.Views;
using FairFlexxApps.Capture.ViewModels;
using FairFlexxApps.Capture.Services.HttpService;
using FairFlexxApps.Capture.Interfaces.HttpService;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Services.LocalDatabase;
using CommunityToolkit.Maui;

using BarcodeScanner.Mobile;
using FairFlexxApps.Capture.Controls.RadioButton;
using FairFlexxApps.Capture.Droid.Controls;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Views.ViewCells.TopMenuViews.TouchTracking;
using FairFlexxApps.Capture.Droid.Services.SQLiteService;
using FairFlexxApps.Capture.Droid.Utilities;

namespace FairFlexxApps.Capture
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
            => MauiApp
                .CreateBuilder()
                .UsePrismApp<App>(PrismStartup.Configure)
                //.UsePrismApp<App>(prism =>
                //{
                //    prism.OnInitialized(container =>
                //    {
                //    })
                //    .OnInitialized(() =>
                //    {
                //    });
                //    prism.RegisterTypes(container =>
                //    {
                //    });
                //    prism.OnAppStart(async (container, navigation) =>
                //    {
                //    });
                //})
                .UseFFImageLoading()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
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
                .ConfigureEffects(effects =>
                {
                    effects.Add<TouchEffect, TouchPlatformEffect>();
                }
                )
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddBarcodeScannerHandler();
#if ANDROID

                    handlers.AddHandler(typeof(CrossButton), typeof(CrossButtonRenderer));
                    handlers.AddHandler(typeof(CustomCheckBox), typeof(CustomCheckBoxRenderer));
                    handlers.AddHandler(typeof(CustomEditor), typeof(CustomEditorRenderer));
                    handlers.AddHandler(typeof(CustomEntry), typeof(CustomEntryRenderer));
                    handlers.AddHandler(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer));
                    handlers.AddHandler(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer));
#endif

                })
                .ConfigureMopups()
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews()
                .Build();

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IHttpRequest, HttpRequest>(); // RegisterServices<IFileService>(new FileService());
            mauiAppBuilder.Services.AddSingleton<ISerializer, Serializer>(); // RegisterServices<IFileService>(new FileService());
            mauiAppBuilder.Services.AddSingleton<ISqLiteService, SqLiteService>(); // RegisterServices<IFileService>(new FileService());
                                                                                   // For android
#if ANDROID
            mauiAppBuilder.Services.AddSingleton<IDatabaseConnection, DatabaseConnection>(); // RegisterServices<IFileService>(new FileService());
            mauiAppBuilder.Services.AddSingleton<IFileService, FileService>(); // RegisterServices<IFileService>(new FileService());
#endif
            // For iOS

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
