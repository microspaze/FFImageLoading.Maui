﻿#if !ANDROID && !WINDOWS && !IOS && !TIZEN && !MACCATALYST
using System;
using FFImageLoading.Mock;
using FFImageLoading.Helpers;
using FFImageLoading.Cache;
using System.Reflection;
using System.Linq;
using FFImageLoading.Work;
using FFImageLoading.Config;

namespace FFImageLoading
{
    /// <summary>
    /// FFImageLoading by Daniel Luberda
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ImageService : ImageServiceBase<MockBitmap>
    {
        const string DoNotReference = "You are referencing the portable assembly - you need to reference the platform specific assembly";
		static IImageService _instance;

		public static IImageService Instance => _instance ??= ServiceHelper.GetService<IImageService>();

		public ImageService(
			IConfiguration configuration,
			 IMD5Helper md5Helper,
			 IMiniLogger miniLogger,
			 IPlatformPerformance platformPerformance,
			 IMainThreadDispatcher mainThreadDispatcher,
			 IDataResolverFactory dataResolverFactory,
			 IDiskCache diskCache,
             IDownloadCache downloadCache,
			 IWorkScheduler workScheduler)
			 : base(configuration, md5Helper, miniLogger, platformPerformance, mainThreadDispatcher, dataResolverFactory, diskCache, downloadCache, workScheduler)
		{
		}

        public override IMemoryCache<MockBitmap> MemoryCache => MockImageCache.Instance;

        public static IImageLoaderTask CreateTask<TImageView>(TaskParameter parameters, ITarget<MockBitmap, TImageView> target) where TImageView : class
        {
            return new PlatformImageLoaderTask<TImageView>(target, parameters, Instance);
        }

        public override IImageLoaderTask CreateTask(TaskParameter parameters)
        {
            return new PlatformImageLoaderTask<object>(null, parameters, this);
        }

        protected override void SetTaskForTarget(IImageLoaderTask currentTask)
        {
            // throw new NotImplementedException();
        }

        public override void CancelWorkForView(object view)
        {
            // throw new NotImplementedException();
        }

        public override int DpToPixels(double dp, double scale)
        {
            return (int)dp;
        }

        public override double PixelsToDp(double px, double scale)
        {
            return px;
        }
    }
}

#endif
