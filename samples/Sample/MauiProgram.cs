using FFImageLoading.Maui;

namespace Sample
{
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

			// Set FFImageLoading Global Configuration
			// Note: If you want to update the configuration later, you can follow the example in BasicPage.xaml.cs
			var ffilConfig = new FFImageLoading.Config.Configuration()
			{
				HttpClient = new HttpClient(new AuthenticatedHttpImageClientHandler(() => "xxxxxx")),
			};
			builder.Services.AddSingleton<FFImageLoading.Config.IConfiguration>(ffilConfig);

			App = builder.Build();

			return App;
		}

		public static MauiApp App { get; private set; }
		public static IServiceProvider Services
			=> App.Services;
	}

	public static class Helpers
	{
		private static readonly bool _useBingImage = true;

		public static string GetImageUrl(int key, int width = 600, int height = 600)
		{
			return _useBingImage ? $"https://bing.img.run/rand_1366x768.php?tag=nature{key}" :  $"https://picsum.photos/seed/nature{key}/{width}/{height}";
		}

		public static string GetRandomImageUrl(int width = 600, int height = 600)
		{
			return _useBingImage ? $"https://bing.img.run/rand_1366x768.php?tag=nature{Guid.NewGuid()}" : $"https://picsum.photos/seed/nature{Guid.NewGuid()}/{width}/{height}";
		}
	}
}
