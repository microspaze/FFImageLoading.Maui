using System;
using System.IO;
using System.Threading.Tasks;
using FFImageLoading.Work;
using FFImageLoading.Helpers;
using FFImageLoading.Extensions;
using FFImageLoading.Config;
using FFImageLoading.Helpers.Gif;
using Windows.Graphics.Imaging;
using Windows.Foundation;
using Microsoft.UI.Xaml.Media.Imaging;

namespace FFImageLoading.Decoders
{
	public class GifDecoder : IDecoder<BitmapHolder>
	{
		public GifDecoder(IImageService imageService)
		{
			ImageService = imageService;
		}

		protected readonly IImageService ImageService;

		public async Task<IDecodedImage<BitmapHolder>> DecodeAsync(Stream imageData, string path, Work.ImageSourceType sourceType, ImageInformation imageInformation, TaskParameter parameters)
		{
			var result = new DecodedImage<BitmapHolder>();
			if (parameters.DownSampleSize == null && (parameters.Transformations == null || parameters.Transformations.Count == 0))
			{
				//Use windows default GIF decoder
				var bitmap = await imageData.ToBitmapImageAsync(ImageService, parameters.Scale, parameters.DownSampleSize, parameters.DownSampleUseDipUnits, parameters.DownSampleInterpolationMode, allowUpscale: false, imageInformation).ConfigureAwait(false);
				result.Image = new BitmapHolder(bitmap);
				return result;
			}

			using (var gifDecoder = new GifHelper())
			{
				var insampleSize = 1;

				// DOWNSAMPLE
				if (parameters.DownSampleSize != null && (parameters.DownSampleSize.Item1 > 0 || parameters.DownSampleSize.Item2 > 0))
				{
					// Calculate inSampleSize
					var downsampleWidth = parameters.DownSampleSize.Item1;
					var downsampleHeight = parameters.DownSampleSize.Item2;

					if (parameters.DownSampleUseDipUnits)
					{
						downsampleWidth = ImageService.DpToPixels(downsampleWidth, parameters.Scale);
						downsampleHeight = ImageService.DpToPixels(downsampleHeight, parameters.Scale);
					}
					await gifDecoder.ReadHeaderAsync(imageData).ConfigureAwait(false);
					insampleSize = CalculateInSampleSize(gifDecoder.Width, gifDecoder.Height, downsampleWidth, downsampleHeight, false);
				}

				await gifDecoder.ReadAsync(imageData, insampleSize).ConfigureAwait(false);
				gifDecoder.Advance();

				imageInformation.SetOriginalSize(gifDecoder.Width, gifDecoder.Height);

				if (insampleSize > 1)
					imageInformation.SetCurrentSize(gifDecoder.DownsampledWidth, gifDecoder.DownsampledHeight);
				else
					imageInformation.SetCurrentSize(gifDecoder.Width, gifDecoder.Height);

				//Build Animated Frames
				result.IsAnimated = gifDecoder.FrameCount > 1 && ImageService.Configuration.AnimateGifs;
				var frameCount = result.IsAnimated ? gifDecoder.FrameCount : 1;
				result.AnimatedImages = new IAnimatedImage<BitmapHolder>[frameCount];
				for (var i = 0; i < frameCount; i++)
				{
					var animatedImage = new AnimatedImage<BitmapHolder>
					{
						Delay = gifDecoder.GetDelay(i),
						Image = await gifDecoder.GetNextFrameAsync().ConfigureAwait(false)
					};
					result.AnimatedImages[i] = animatedImage;
					gifDecoder.Advance();
				}

				if (result.AnimatedImages != null
					&& result.AnimatedImages.Length != 0
					&& result.AnimatedImages[0].Image != null)
				{
					imageInformation.SetOriginalSize(result.AnimatedImages[0].Image.Width, result.AnimatedImages[0].Image.Height);
					imageInformation.SetCurrentSize(result.AnimatedImages[0].Image.Width, result.AnimatedImages[0].Image.Height);
				}

				return result;
			}
		}

		public static int CalculateInSampleSize(int sourceWidth, int sourceHeight, int reqWidth, int reqHeight, bool allowUpscale)
		{
			// Raw height and width of image
			float width = sourceWidth;
			float height = sourceHeight;

			if (reqWidth == 0)
				reqWidth = (int)((reqHeight / height) * width);

			if (reqHeight == 0)
				reqHeight = (int)((reqWidth / width) * height);

			var inSampleSize = 1;

			if (height > reqHeight || width > reqWidth || allowUpscale)
			{
				// Calculate ratios of height and width to requested height and width
				var heightRatio = (int)Math.Round(height / reqHeight);
				var widthRatio = (int)Math.Round(width / reqWidth);

				// Choose the smallest ratio as inSampleSize value, this will guarantee
				// a final image with both dimensions larger than or equal to the
				// requested height and width.
				inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
			}

			return inSampleSize;
		}

		public class GifHelper : GifHelperBase<BitmapHolder>
		{
			protected override BitmapHolder GetNextBitmap()
			{
				return new BitmapHolder(null, DownsampledWidth, DownsampledHeight);
			}

			protected override void SetPixels(BitmapHolder bitmap, int[] pixels, int width, int height)
			{
				bitmap.SetPixels(pixels, width, height);
			}

			protected override void GetPixels(BitmapHolder bitmap, int[] pixels, int width, int height)
			{
				bitmap.GetPixels(pixels, width, height);
			}

			protected override void Release(BitmapHolder bitmap)
			{
				bitmap?.FreePixels();
			}
		}
	}
}
