# FFImageLoading.Maui - Fast & Furious Image Loading for .NET MAUI


Forked from the amazingly popular [original FFImageLoading Library](https://github.com/luberda-molinet/FFImageLoading), this *Compat* version [FFImageLoading.Compat](https://github.com/Redth/FFImageLoading.Compat) aims to ease your migration from Xamarin.Forms to .NET MAUI with a compatible implementation to get you up and running without rewriting the parts of your app that relied on the original library.

This *Maui* version which merges all Transformations & SVG library parts into ONE and is migrated from *FFImageLoading.Compat* aims to fix some critical bugs and gives you a place to submit Maui releated issues.

## The Most *CRITICAL* bugs it aims to fix：

1. [Fixed in 1.0.1][iOS] App Reloading Bug. (When a page contains a CachedImage which use local image file as LoadingPlaceholder, after tombstone the app to background and then recover it to forground, the app will be reloaded to the home page)
2. [Fixed in 1.0.3][iOS] Google webp format image support. (It works in Xamarin.Forms version, but not in FFImageLoading.Compat)
3. [Fixed in 1.0.7][Windows] Local images files will not show on Windows.


*Thanks to the Original Authors: Daniel Luberda, Fabien Molinet & Redth.*

## Usage

1. Install NuGet package: [FFImageLoading.Maui](http://www.nuget.org/packages/FFImageLoading.Maui) [![NuGet](https://img.shields.io/nuget/v/FFImageLoading.Maui.svg?label=NuGet)](https://www.nuget.org/packages/FFImageLoading.Maui)
2. Add `.UseFFImageLoading()` to your MAUI app builder.

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseFFImageLoading()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
```

3. Add `xmlns:ffimageloading="clr-namespace:FFImageLoading.Maui;assembly=FFImageLoading.Maui"` to your MAUI Xaml page references.
4. Add `<ffimageloading:CachedImage Source="xxx.jpg"></ffimageloading:CachedImage>` to display image.


## Support platforms

- [x] Android      
- [x] iOS          
- [x] MacCatalyst
- [x] Windows

## Features

- .NET MAUI (iOS, MacCatalyst, Android, Windows) support
- Configurable disk and memory caching
- Multiple image views using the same image source (url, path, resource) will use only one bitmap which is cached in memory (less memory usage)
- Deduplication of similar download/load requests. *(If 100 similar requests arrive at same time then one real loading will be performed while 99 others will wait).*
- Error and loading placeholders support
- Images can be automatically downsampled to specified size (less memory usage)
- Fluent API which is inspired by Picasso naming
- SVG / WebP / GIF support
- Image loading Fade-In animations support
- Can retry image downloads (RetryCount, RetryDelay)
- Android bitmap optimization. Saves 50% of memory by trying not to use transparency channel when possible.
- Transformations support
  - BlurredTransformation
  - CircleTransformation, RoundedTransformation, CornersTransformation, CropTransformation
  - ColorSpaceTransformation, GrayscaleTransformation, SepiaTransformation, TintTransformation
  - FlipTransformation, RotateTransformation
  - Supports custom transformations (native platform `ITransformation` implementations)

## Original Library Documentation

[Xamarin]https://github.com/luberda-molinet/FFImageLoading

[Maui]https://github.com/Redth/FFImageLoading.Compat

## Release Notes

## 1.1.0

1.Fix Windows Application start up crash when DispatcherQueue in MainThreadDispatcher is null.

## 1.0.9

1.Fix Local file images do not appear when DownsampleToViewSize is set to true (Thanks Reported by https://github.com/CraigTenn)

## 1.0.8

1.Fix AspectFill images with an implicit height (Thanks PR by https://github.com/brendan-holly-modea)

## 1.0.7

1.Use Svg.Skia instead of custom code. (Thanks PR by https://github.com/PavloLukianets)

2.Fix Android MemoryCache re-creation issue. (Thanks PR by https://github.com/MichaelFrenkel)

3.Fixed NRE on ImageService caused before ready. (Thanks PR by https://github.com/NishiokaTakeo)

4.Fixed Local images files will not show on Windows. (Thanks Reported by https://github.com/Inrego)

5.Fixed AspectFill not working on Android for CachedImage control. (Thanks Reported by https://github.com/brendan-holly-modea)

6.Add Sample.Win project file.

## 1.0.5

1.Add .NET 8 support.

## 1.0.3

1.Fix Webp image not showing Bug.

## 1.0.1

1.Fix App Reloading Bug.

