﻿using System;
using System.Threading.Tasks;
using FFImageLoading.Work;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using FFImageLoading.Targets;
using FFImageLoading.Extensions;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace FFImageLoading
{
    /// <summary>
    /// TaskParameterPlatformExtensions
    /// </summary>
    public static class TaskParameterPlatformExtensions
    {
        /// <summary>
        /// Loads the image into PNG Stream
        /// </summary>
        /// <returns>The PNG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static async Task<Stream> AsPNGStreamAsync(this TaskParameter parameters, IImageService imageService)
        {
            var result = await AsWriteableBitmapAsync(parameters, imageService).ConfigureAwait(false);
            var stream = await result.AsPngStreamAsync().ConfigureAwait(false);

            return stream;
        }

        /// <summary>
        /// Loads the image into JPG Stream
        /// </summary>
        /// <returns>The JPG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static async Task<Stream> AsJPGStreamAsync(this TaskParameter parameters, IImageService imageService, int quality = 80)
        {
            var result = await AsWriteableBitmapAsync(parameters, imageService).ConfigureAwait(false);
            var stream = await result.AsJpegStreamAsync(quality).ConfigureAwait(false);

            return stream;
        }

        /// <summary>
        /// Loads and gets WriteableBitmap using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>The WriteableBitmap.</returns>
        /// <param name="parameters">Parameters.</param>
        public static Task<WriteableBitmap> AsWriteableBitmapAsync(this TaskParameter parameters, IImageService imageService)
        {
            var target = new BitmapTarget();
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<WriteableBitmap>();

            parameters
                .Error(ex =>
                {
                    tcs.TrySetException(ex);
                    userErrorCallback?.Invoke(ex);
                })
                .Finish(scheduledWork =>
                {
                    finishCallback?.Invoke(scheduledWork);
                    tcs.TrySetResult(target.BitmapSource as WriteableBitmap);
                });

            if (parameters.SourceType != Work.ImageSourceType.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.TryDispose();
                return null;
            }

            var task = ImageService.CreateTask(parameters, target);
            imageService.LoadImage(task);

            return tcs.Task;
        }

        /// <summary>
        /// Loads the image into given Image using defined parameters.
        /// </summary>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static IScheduledWork Into(this TaskParameter parameters, Image imageView, IImageService imageService)
        {
            var target = new ImageTarget(imageView);
            return parameters.Into<Image>(target, imageService);
        }

        /// <summary>
        /// Loads the image into given Image using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>An awaitable Task.</returns>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Image imageView, IImageService imageService)
        {
            return parameters.IntoAsync(param => param.Into(imageView, imageService));
        }

        /// <summary>
        /// Loads the image into given target using defined parameters.
        /// </summary>
        /// <returns>The into.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="target">Target.</param>
        /// <typeparam name="TImageView">The 1st type parameter.</typeparam>
        public static IScheduledWork Into<TImageView>(this TaskParameter parameters, ITarget<BitmapSource, TImageView> target, IImageService imageService) where TImageView : class
        {
            if (parameters.SourceType != ImageSourceType.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.TryDispose();
                return null;
            }

            var task = ImageService.CreateTask(parameters, target);
            imageService.LoadImage(task);
            return task;
        }

        /// <summary>
        /// Loads the image into given target using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="target">Target.</param>
        /// <typeparam name="TImageView">The 1st type parameter.</typeparam>
        public static Task<IScheduledWork> IntoAsync<TImageView>(this TaskParameter parameters, ITarget<BitmapSource, TImageView> target, ImageService imageService) where TImageView : class
        {
            return parameters.IntoAsync(param => param.Into(target, imageService));
        }

        private static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Action<TaskParameter> into)
        {
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<IScheduledWork>();

            parameters
                .Error(ex => {
                    tcs.TrySetException(ex);
                    userErrorCallback?.Invoke(ex);
                })
                .Finish(scheduledWork => {
                    finishCallback?.Invoke(scheduledWork);
                    tcs.TrySetResult(scheduledWork);
                });

            into(parameters);

            return tcs.Task;
        }
    }
}
