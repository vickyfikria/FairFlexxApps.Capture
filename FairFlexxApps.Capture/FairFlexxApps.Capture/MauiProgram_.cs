namespace FairFlexxApps.Capture;

public static class MauiProgram_
{

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
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

            //    }
            //    );

            //    prism.OnAppStart(async (container, navigation) =>
            //    {

            //    }

            //    );
            //})

            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


            //.ConfigureMopups()
            //.UseSkiaSharp()
            //.UseFFImageLoading()
            //.ConfigureMauiHandlers(handlers =>
            //{
            //    handlers.AddBarcodeScannerHandler(); // nuget BarcodeScanner with Google Vision API
            //    //handlers.AddCompatibilityRenderer()
            //});


        return builder.Build();
    }






}
