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
4. Add `<ffimageloading:CachedImage Source="xxx.jpg"></ffimageloading:CachedImage>` to display image. Or add `<ffimageloading:CachedImageView Source="xxx.jpg" Stroke="Red" StrokeThickness="1"></ffimageloading:CachedImageView>` to display image with border.
5. **If you're facing with some weird issues, try to clean the whole solution or upgrade your vs and maui to the latest version.**


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


## License
The MIT License (MIT) see [License file](LICENSE.md)


## Thanks
Thank JetBrains for providing DEV tools in developing. (Especially on MacOS)

![avatar](samples/Sample/Resources/Images/jetbrains_logo.png)

## Release Notes

## 1.3.1

1.Update Svg.Skia lib to 3.0.4 to support Android 16K page size.

2.Merge PR Prevent DI Configuration HttpClient from being overwritten on init #57 (Thanks PR by https://github.com/GuidoNeele)

## 1.2.9

1.Update Svg.Skia lib to 2.0.0.8

## 1.2.8

1.Add .NET 9 support.

## 1.2.7

1.Fix iOS & Mac ColorSpaceTransformation cause app crush bug.

2.Fix #44 Windows CachedImage handler is failing when application is being closed. (Thanks PR by https://github.com/pedroafsouza) 

3.Fix #47 NSInternalInconsistencyException when loading certain animated images and applying a transform. (Thanks Reported by https://github.com/kpespisa)

4.Fix #48 OnLoadingPlaceholderSet Memory Leak. (Thanks PR by https://github.com/MichaelFrenkel) 

## 1.2.6

1.Fix Unit Test Error XFC0000 Cannot resolve type "clr-namespace:...fftransformations:GrayscaleTransformation" when targets to pure .net framework #33 (Thanks Reported by https://github.com/VegardAaberge)

## 1.2.5

1.Fix retry image download is not working. #36 (Thanks Reported by https://github.com/BoungSeokKim)

## 1.2.4

1.Fix Android app crashes when using using multiple CachedImage for showing multiple gifs #35 (Thanks Reported by https://github.com/Th3L0x)

## 1.2.3

1.Fix CropTransformation makes the image bigger #32 (iOS) (Thanks Reported by https://github.com/eddieyanez)

## 1.2.1

1.Fix SVGs on iOS are blurry #31 (iOS) (Thanks Reported by https://github.com/PavloLukianets)

## 1.2.0

1.Fix InvalidateCacheEntryAsync gives a null pointer exception #27 (All Platforms) (Thanks Reported by https://github.com/apoorvadixit-ttc)

## 1.1.9

1.Add new bordered CachedImage control CachedImageView (eg. Sample/SampleGifPage.xaml).

2.Refine fix for issue #24 Android Potential race condition during loading of image source and LoadingPlaceholder (If placeholder is GIF and image is also GIF, then old fix will potential cause image load failed)

3.Improve GIF image's loading performance by change for loop to Parallel.ForEach loop.

4.Fix Transparent GIF images display with DARK background issue (Windows).

## 1.1.8

1.Fix Customising the HttpClient does not seem to have any effect #26 (All Platforms) (Thanks Reported by https://github.com/apoorvadixit-ttc)

## 1.1.6

1.Fix Potential race condition during loading of image source and LoadingPlaceholder #24 (Android) (Thanks Reported by https://github.com/EvgenyMuryshkin)

## 1.1.5

1.Fix Broken CachedImage scaling on Android #22 (Sync to iOS/MacOs/Windows) (Thanks Reported by https://github.com/rafalka)

2.Add GIF & Webp image support for Windows. (Transparent background GIF currently unsupported. HELP NEEDED! Resolved in 1.1.9)

## 1.1.2

1.Fix Android app is continue to load the same image. #21 (Thanks Reported by https://github.com/BoungSeokKim)

## 1.1.1

1.Fix DiskCache is null within the ImageService object. (Thanks Reported by https://github.com/kpespisa)

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
