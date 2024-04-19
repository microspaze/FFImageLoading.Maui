# FFImageLoading.Maui - Fast & Furious Image Loading for .NET MAUI, .NET IOS, .NET ANDROID
This is the same package as FFImageLoading.Maui. But I want to use it for .Net ios & .Net android project which doesn't look possible with the FFImageLoading.Maui.

Please download the nuget from here.
https://www.nuget.org/packages/FFImage.Loading.Net.Ios.Android

Here is the example how you can use it in .Net ios project.
Put this kind of code in your AppDelegate file.

```csharp
private void InitMauiContext()
{
    var builder = MauiApp.CreateBuilder();
    builder.UseMauiEmbedding<Microsoft.Maui.Controls.Application>();
    builder.UseFFImageLoading();
    builder.ConfigureEssentials(essentials =>
    {
        essentials.UseVersionTracking();
    })
    builder.Services.Add(new ServiceDescriptor(typeof(UIWindow), Window))
    MauiApp mauiApp = builder.Build();
    var context = new MauiContext(mauiApp.Services)
    //Prepare for FFImageLoading libray.
    ServiceHelper.Init(mauiApp.Services);
}
```

Please do the similar things in .Net android. The important thing is to add  
ServiceHelper.Init(mauiApp.Services);

Other than that, Please follow the same documentation from 
https://github.com/microspaze/FFImageLoading.Maui