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

			App = builder.Build();

			return App;
		}

		public static MauiApp App { get; private set; }
		public static IServiceProvider Services
			=> App.Services;
	}

	public static class Helpers
	{
		public static string GetImageUrl(int key, int width = 600, int height = 600)
		{
			return $"https://picsum.photos/seed/nature{key}/{width}/{height}";
		}

		public static string GetRandomImageUrl(int width = 600, int height = 600)
		{
			return $"https://picsum.photos/seed/nature{Guid.NewGuid()}/{width}/{height}";
		}
	}
}
