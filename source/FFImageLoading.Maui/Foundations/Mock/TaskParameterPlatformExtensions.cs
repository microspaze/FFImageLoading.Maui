#if !ANDROID && !WINDOWS && !IOS && !TIZEN && !MACCATALYST
using System;
using System.IO;
using System.Threading.Tasks;
using FFImageLoading.Mock;
using FFImageLoading.Work;

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
			return null;
        }

        /// <summary>
        /// Loads the image into JPG Stream
        /// </summary>
        /// <returns>The JPG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static async Task<Stream> AsJPGStreamAsync(this TaskParameter parameters, IImageService imageService, int quality = 80)
        {
			return null;
        }
    }
}

#endif
