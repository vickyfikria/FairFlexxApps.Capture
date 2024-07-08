# .NET MAUI conversion

Conversion guidance mostly taken from :

1. [econtainerShop migration guidance](https://github.com/dotnet-architecture/eshop-mobile-client/blob/main/migration.md)
2. [Official migration Xamarin Forms to .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/migration/skiasharp?view=net-maui-8.0)

## Prism Template Pack 
This source is created from Prism template
1. From Visual Studio 2022 , goto Nuget then select Prism Template Pack. 
2. Restart Visual Studio
3. Select New Project with Prism .NET MAUI (Dan Siegel)
4. Then change <TargetFramework> of .csproj file from net6.0-xxx to net8.0-xxx, example net6.0-android to net8.0-android
5. Clean Build

