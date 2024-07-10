# .NET MAUI conversion

Conversion guidance mostly taken from :

1. [econtainerShop migration guidance](https://github.com/dotnet-architecture/eshop-mobile-client/blob/main/migration.md)
2. [Official migration Xamarin Forms to .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/migration/skiasharp?view=net-maui-8.0)
3. [Reusing Effects](https://github.com/dotnet/maui/wiki/Migrating-Xamarin.Forms-Effects)
4. [Custom Renderer](https://learn.microsoft.com/en-us/dotnet/maui/migration/renderer-to-handler?view=net-maui-8.0)

## Color.Accent
Xamarin.Forms code : `Color.Accent`
.NET MAUI : N/A. But in this code will use `Color.FromArgb("FF4081")`


## Prism Template Pack 
This source is created from Prism template
1. From Visual Studio 2022 , goto Nuget then select Prism Template Pack. 
2. Restart Visual Studio
3. Select New Project with Prism .NET MAUI (Dan Siegel)
4. Then change <TargetFramework> of .csproj file from net6.0-xxx to net8.0-xxx, example net6.0-android to net8.0-android
5. Clean Build

## Exclusion
Exclude/ignore folders below from csproj FairFlexxApps.Capture : 
1. All .cs file in : FairFlexxApps.Capture.Android.FotoScanSDK
2. All axmls file in : FairFlexxApps.Capture.Android.Resource.layout, except Tabbar.axml, Toolbar.axml
3. FotoScanSDKImplementation file in : FairFlexxApps.Capture.Service

## Nuget Xamarin Form replacement
Rgs.Plugin -> Mopups

SkiaSharp.Forms -> SkiaSharp, SkiaSharp.Views.Maui.Controls, SkiaSharp.Views.Maui.Core

FFImageLoading -> FFImageLoadingCompat.Maui

Prism.Forms -> Prism.DryIoc.Maui

GoogleVisionBarCode -> BarcodeScanner.Mobile.Maui

AppCenter -> same

SignaturePad -> ?

OpenCV (?) -> Xamarin.OpenCV.Droid

Plugin.Connectivity -> CommunityToolkit.Maui

Plugin.SecureStorage -> CommunityToolkit.Maui

vcardreader (?) -> VCardReader

## SignaturePad 
The Old Xamarin source is implementing SignaturePad Nuget. This SignaturePad is no longer maintain and didn't have MAUI version.  CommunityToolkit.Maui has Drawing

## Cutom renderer

Based on [this](https://learn.microsoft.com/en-us/dotnet/maui/migration/renderer-to-handler?view=net-maui-8.0) we reuse Xamarin forms Custom renderer.

## Permissions
[reference](https://www.youtube.com/watch?v=9GljgwfpiiE&t=907s)
```
    var camerastatus = PermissionStatus.Unknown;

    camerastatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
    if (camerastatus == PermissionStatus.Granted)
        return;

    if (Permissions.ShouldShowRationale<Permissions.Camera>())
    {
        await MessagePopup.Instance.Show(message: TranslateExtension.Get("GrantPermissionCamera"),
            closeButtonText: "OK", textBackgroundColor: "#bdbdbd",
            closeCommand: ((ScanQrCodePageViewModel)this.BindingContext).BackCommand);
    }

    camerastatus = await Permissions.RequestAsync<Permissions.Camera>();
    //old
    //bool allowed = await CheckCameraPermission();
    //if(!allowed)

    if (camerastatus != PermissionStatus.Granted)
    {
        await MessagePopup.Instance.Show(message: TranslateExtension.Get("GrantPermissionCamera"),
            closeButtonText: "OK", textBackgroundColor: "#bdbdbd",
            closeCommand: ((ScanQrCodePageViewModel)this.BindingContext).BackCommand);
    }
```
