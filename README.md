# .NET MAUI conversion
> This is FairFlexxApps.Capture .NET MAUI version

Conversion items :

0. [migration from Android Xamarin to AndroidX ](https://purple.telstra.com/blog/migrate-xamarin-android-androidx)
1. [econtainerShop migration guidance](https://github.com/dotnet-architecture/eshop-mobile-client/blob/main/migration.md)
2. [Official migration Xamarin Forms to .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/migration/skiasharp?view=net-maui-8.0)
3. [Reusing Effects](https://github.com/dotnet/maui/wiki/Migrating-Xamarin.Forms-Effects)
4. [Custom Renderer](https://learn.microsoft.com/en-us/dotnet/maui/migration/renderer-to-handler?view=net-maui-8.0)
5. [Touch Effect](https://learn.microsoft.com/en-us/answers/questions/975093/trying-to-migrate-xamarin-forms-effect-functionali)
6. [Create BindableProperty](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/bindable-properties?view=net-maui-8.0)
7. [Java compilation error : Type AndroidX.Activity.ViewModelLazyKt define multiple times ](https://github.com/xamarin/AndroidX/issues/764)
8. [Content Provider exception](https://github.com/jamesmontemagno/MediaPlugin)
>Unable to get provider android.support.v4.content.FileProvider: java.lang.ClassNotFoundException: Didn't find class "android.support.v4.content.FileProvider" on path: DexPathList
9. [Android 14 CrossMedia.TakePhotoAsync merged to CommunityToolKit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/camera-view?tabs=android)
10. [Secure storage .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage?view=net-maui-8.0&tabs=android)
11. [CrossConnectivity changed to Connectivity](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/networking?view=net-maui-8.0&tabs=android)
12. [Prism always fail to resolve ISQLiteService injection - no reference of db3 file](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/8.0/getfolderpath-unix#android)
13. [ScrollView that inside Stacklayout didn't work in .NET MAUI](https://github.com/dotnet/maui/issues/12452)

##13 ScrollView that inside Stacklayout didn't work in .NET MAUI
Need to add vertical options = FillAndExpand `<ScrollView VerticalOptions="FillAndExpand">`


##12 Prism always fail to resolve ISQLiteService injection - no reference of db3 file
[Source](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/8.0/getfolderpath-unix#android)
Change Environment.SpecialFolder.Personal to Environment.SpecialFolder.UserProfile


##11 CrossConnectivity for checking connection availableness

This is taken from UploadLeadListPageViewModel
```
    // Cross Connectivity : X, change to Connectivity
    Connectivity.Current.ConnectivityChanged += (s, e) =>
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            // Connection to internet is available
            HttpRequest.CancelTokenSource();
        }

        // CrossConnectivity.Current.ConnectivityChanged += (s, e) =>
        //{
        //  if (!CrossConnectivity.Current.IsConnected)
        //  {
        //    HttpRequest.CancelTokenSource();
        //  }
        //}
    };
```                

#Conversion Details

## Migration to androidx
It is very important step. Visual Studio has provided AndroidX migration(experimental).


## Color.Accent
Xamarin.Forms code : `Color.Accent`
.NET MAUI : N/A. But in this code will use `Color.FromArgb("FF4081")`
Xamarin.Forms.Color.R -> Color.Red
Xamarin.Forms.Color.G -> Color.Green
Xamarin.Forms.Color.B -> Color.Blue
Xamarin.Forms.Color.A -> Color.Alpha
Example :
`   if ((color.R + color.G + color.B) >= 1.8)
        return Colors.Black;
`
Change to
`   if ((color.Red + color.Green + color.Blue) >= 1.8)
        return Colors.Black;
`

## Prism Template Pack for .NET MAUI 8
This source is created from Prism template. Official Prism template basics is for .NET MAUI 6. Below change required for .NET MAUI 8
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

AppCenter -> no change

SignaturePad -> ?

OpenCV -> Xamarin.OpenCV.Droid (author : Naxam)

Plugin.Connectivity -> CommunityToolkit.Maui

Plugin.SecureStorage -> CommunityToolkit.Maui

vcardreader (?) -> VCardReader

## SignaturePad 
The Old Xamarin source is implementing SignaturePad Nuget. This SignaturePad is no longer maintain and didn't have MAUI version.  CommunityToolkit.Maui has Drawing

## Cutom renderer

Based on [this](https://learn.microsoft.com/en-us/dotnet/maui/migration/renderer-to-handler?view=net-maui-8.0) we reuse Xamarin forms Custom renderer.

## Permissions pattern for .NET MAUI
Permissions.CheckStatusAsync<>, ShouldShowRationale<>, RequestAsync<> already optimized for multi platform

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
## Touch Effect or Touch Behaviour

## Create BindableProperty : AutoCompleteView
Pattern of [BindableProperty Create](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/bindable-properties?view=net-maui-8.0) 

Before :
`public static readonly BindableProperty PlaceholderProperty = 
BindableProperty.Create<AutoCompleteView, string>(p => p.Placeholder, string.Empty, BindingMode.TwoWay, null, PlaceHolderChanged);`

After :
```        
    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder),
            typeof(string),
            typeof(AutoCompleteView),
            defaultValue :  string.Empty,
            defaultBindingMode : BindingMode.TwoWay,
            propertyChanged: PlaceholderPropertyChanged);

    private static void PlaceholderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var autoCompleteView = bindable as AutoCompleteView;
        if (autoCompleteView != null)
        {
            autoCompleteView._entText.Placeholder = newValue.ToString();
        }
    }
```
## Java compilation error : Type AndroidX.Activity.ViewModelLazyKt define multiple times 
Based on this [reference](https://github.com/xamarin/AndroidX/issues/764) if compilation error happened, need to add transitive package.
So we need to install nuget Xamarin.AndroidX.Activity.Ktx version 1.9.0.3

## Unable to get provider android.support.v4.content.FileProvider
If ClassNotFoundException exception happened because of android.support.v4.content.FileProvider, in AndroidManifest  need to change 
`<provider android:name=android.support.v4.content.FileProvider` to
 `<provider android:name="androidx.core.content.FileProvider"`

> Unable to get provider android.support.v4.content.FileProvider: java.lang.ClassNotFoundException: Didn't find class "android.support.v4.content.FileProvider" on path: DexPathList
```
<provider android:name="androidx.core.content.FileProvider" 
          android:authorities="${applicationId}.fileprovider" 
          android:exported="false" 
          android:grantUriPermissions="true">
          
	  <meta-data android:name="android.support.FILE_PROVIDER_PATHS" 
                     android:resource="@xml/file_paths"></meta-data>
</provider>
```


## Secure storage 
Write a Value 
`await SecureStorage.Default.SetAsync("oauth_token", "secret-oauth-token-value");`
Read a Value 
`string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");

if (oauthToken == null)
{
    // No value is associated with the key "oauth_token"
}`

Remove a value 
To remove a specific value, remove the key
`bool success = SecureStorage.Default.Remove("oauth_token");`
To remove all values, use the `RemoveAll` method:
`SecureStorage.Default.RemoveAll();`
