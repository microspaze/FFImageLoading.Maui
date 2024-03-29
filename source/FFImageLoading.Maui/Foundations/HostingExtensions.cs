﻿using FFImageLoading.Cache;
using FFImageLoading.Work;
using FFImageLoading.Helpers;
using FFImageLoading.DataResolvers;
using FFImageLoading.Config;

namespace FFImageLoading
{
	public static class HostingExtensions
	{
		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddSingleton<IMD5Helper, MD5Helper>();
			services.AddSingleton<IMiniLogger, MiniLogger>();
			services.AddSingleton<IDiskCache, SimpleDiskCache>();
			services.AddSingleton<IConfiguration, Configuration>();
			services.AddSingleton<IDownloadCache, DownloadCache>();
			services.AddSingleton<IWorkScheduler, WorkScheduler>();

#if IOS || MACCATALYST
			services.AddSingleton<IImageService, ImageService>();
#elif ANDROID
			services.AddSingleton<IImageService, ImageService>();
#elif WINDOWS
			services.AddSingleton<IImageService, ImageService>();
#endif

#if ANDROID || WINDOWS || IOS || MACCATALYST || TIZEN
			services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();
			services.AddSingleton<IPlatformPerformance, PlatformPerformance>();
			services.AddSingleton<IDataResolverFactory, DataResolverFactory>();
#endif
		}
	}
}
