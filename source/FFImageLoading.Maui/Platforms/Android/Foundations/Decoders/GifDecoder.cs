﻿using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using FFImageLoading.Work;
using FFImageLoading.Helpers;
using FFImageLoading.Extensions;
using FFImageLoading.Config;
using FFImageLoading.Helpers.Gif;
using FFImageLoading.Drawables;

namespace FFImageLoading.Decoders
{
	public class GifDecoder : IDecoder<Bitmap>
	{
		public GifDecoder(IImageService imageService)
		{
			ImageService = imageService;
		}

		protected readonly IImageService ImageService;

		public async Task<IDecodedImage<Bitmap>> DecodeAsync(Stream stream, string path, Work.ImageSourceType sourceType, ImageInformation imageInformation, TaskParameter parameters)
		{
			var result = new DecodedImage<Bitmap>();

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
					await gifDecoder.ReadHeaderAsync(stream).ConfigureAwait(false);
					insampleSize = BaseDecoder.CalculateInSampleSize(gifDecoder.Width, gifDecoder.Height, downsampleWidth, downsampleHeight, false);
				}

				await gifDecoder.ReadAsync(stream, insampleSize).ConfigureAwait(false);
				gifDecoder.Advance();

				imageInformation.SetOriginalSize(gifDecoder.Width, gifDecoder.Height);

				if (insampleSize > 1)
					imageInformation.SetCurrentSize(gifDecoder.DownsampledWidth, gifDecoder.DownsampledHeight);
				else
					imageInformation.SetCurrentSize(gifDecoder.Width, gifDecoder.Height);

				result.IsAnimated = gifDecoder.FrameCount > 1 && ImageService.Configuration.AnimateGifs;

				if (result.IsAnimated && ImageService.Configuration.AnimateGifs)
				{
					result.AnimatedImages = new AnimatedImage<Bitmap>[gifDecoder.FrameCount];

					for (var i = 0; i < gifDecoder.FrameCount; i++)
					{
						var animatedImage = new AnimatedImage<Bitmap>
						{
							Delay = gifDecoder.GetDelay(i),
							Image = await gifDecoder.GetNextFrameAsync().ConfigureAwait(false)
						};
						result.AnimatedImages[i] = animatedImage;

						gifDecoder.Advance();
					}
				}
				else
				{
					result.IsAnimated = false;
					result.Image = await gifDecoder.GetNextFrameAsync().ConfigureAwait(false);
				}



				if (result.Image != null)
				{
					imageInformation.SetOriginalSize(result.Image.Width, result.Image.Height);
					imageInformation.SetCurrentSize(result.Image.Width, result.Image.Height);
				}
				else if (result.AnimatedImages != null)
				{
					if (result.AnimatedImages.Length > 0)
					{
						if (result.AnimatedImages[0].Image != null)
						{
							imageInformation.SetOriginalSize(result.AnimatedImages[0].Image.Width, result.AnimatedImages[0].Image.Height);
							imageInformation.SetCurrentSize(result.AnimatedImages[0].Image.Width, result.AnimatedImages[0].Image.Height);
						}
					}
				}

				return result;
			}
		}

		public class GifHelper : GifHelperBase<Bitmap>
		{
			protected override Bitmap GetNextBitmap()
			{
				var config = IsFirstFrameTransparent == null || IsFirstFrameTransparent.Value
					? Bitmap.Config.Argb8888 : Bitmap.Config.Rgb565;
				var result = Bitmap.CreateBitmap(DownsampledWidth, DownsampledHeight, config);
				result.HasAlpha = config == Bitmap.Config.Argb8888;
				return result;
			}

			protected override void GetPixels(Bitmap bitmap, int[] pixels, int width, int height)
			{
				bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
			}

			protected override void Release(Bitmap bitmap)
			{
				bitmap?.Recycle();
			}

			protected override void SetPixels(Bitmap bitmap, int[] pixels, int width, int height)
			{
				bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);
			}
		}
	}
}
