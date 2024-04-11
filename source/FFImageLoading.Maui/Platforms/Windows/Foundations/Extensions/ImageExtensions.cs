using FFImageLoading.Helpers;
using FFImageLoading.Work;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using System.Text;

namespace FFImageLoading.Extensions
{
    public static class ImageExtensions
    {
		//https://learn.microsoft.com/zh-tw/windows/win32/wic/-wic-native-image-format-metadata-queries
		//https://giflib.sourceforge.net/whatsinagif/bits_and_bytes.html
		//https://giflib.sourceforge.net/whatsinagif/animation_and_transparency.html
		//private static readonly string[] _imageProperties = ["/appext", "/appext/Application", "/appext/Data", "/logscrdesc", "/logscrdesc/Signature", "/logscrdesc/Width", "/logscrdesc/Height", "/logscrdesc/GlobalColorTableFlag", "/logscrdesc/ColorResolution", "/logscrdesc/SortFlag", "/logscrdesc/GlobalColorTableSize", "/logscrdesc/BackgroundColorIndex", "/logscrdesc/PixelAspectRatio"];
		//private static readonly string[] _frameProperties = ["/imgdesc", "/imgdesc/Left", "/imgdesc/Top", "/imgdesc/Width", "/imgdesc/Height", "/imgdesc/LocalColorTableFlag", "/imgdesc/InterlaceFlag", "/imgdesc/SortFlag", "/imgdesc/LocalColorTableSize", "/grctlext", "/grctlext/Delay", "/grctlext/Disposal", "/grctlext/UserInputFlag", "/grctlext/TransparencyFlag", "/grctlext/TransparentColorIndex"];
		private static readonly string _imgdescKey = "/imgdesc";
		private static readonly string _grcelextDelayKey = "/grctlext/Delay";
		private static readonly string _grcelextDisposalKey = "/grctlext/Disposal";
		private static readonly string[] _framePropertyKeys = [ _imgdescKey, _grcelextDelayKey, _grcelextDisposalKey, "/grctlext/UserInputFlag", "/grctlext/TransparencyFlag", "/grctlext/TransparentColorIndex" ];
		private static readonly BitmapPropertySet _appextProperties = new()
		{
			{ "/appext/Application", new BitmapTypedValue(Encoding.UTF8.GetBytes("NETSCAPE2.0"), PropertyType.UInt8Array) }, //Required
			{ "/appext/Data", new BitmapTypedValue(new byte[] { 3, 1, 0, 0 }, PropertyType.UInt8Array) }, //Loop
		};

		public static async Task<BitmapImage> ToAnimatedImageAsync(this IDecodedImage<BitmapHolder> decoded, IMainThreadDispatcher mainThreadDispatcher)
		{
			if (decoded.AnimatedImages == null || decoded.AnimatedImages.Length == 0)
				return null;

			BitmapImage bitmapImage = null;

			await mainThreadDispatcher.PostAsync(async () =>
			{
				bitmapImage = await decoded.ToAnimatedImage();
			}).ConfigureAwait(false);

			return bitmapImage;
		}

		private static async Task<BitmapImage> ToAnimatedImage(this IDecodedImage<BitmapHolder> decoded)
		{
			var bitmapImage = new BitmapImage();
			var animatedImages = decoded.AnimatedImages;
			if (animatedImages == null || animatedImages.Length == 0)
			{
				return bitmapImage;
			}
			var frameCount = animatedImages.Length;
			var frameDelays = new int[frameCount];

			#region 1.Set Frame's Pixels

			var imageStream = new InMemoryRandomAccessStream();
			var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, imageStream);
			await encoder.BitmapContainerProperties.SetPropertiesAsync(_appextProperties);
			for (var i = 0; i < frameCount; i++)
			{
				var animatedImage = animatedImages[i];
				var frameDelay = animatedImage.Delay;
				var frameHolder = animatedImage.Image;
				var frameData = frameHolder.PixelData;
				frameDelays[i] = frameDelay;
				//WARNING:SET FRAME'S DELAY PROPERTY HERE WILL CAUSE DARK BACKGROUND AND NO TRANSPARENCY !!!
				//var frameProperties = new Dictionary<string, BitmapTypedValue>() { { "/grctlext/Delay", new BitmapTypedValue(animatedImage.Delay / 10, PropertyType.UInt16) }};
				//await encoder.BitmapProperties.SetPropertiesAsync(frameProperties);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)frameHolder.Width, (uint)frameHolder.Height, 96, 96, frameData);
				if (i != animatedImages.Length - 1)
				{
					await encoder.GoToNextFrameAsync();
				}
			}
			await encoder.FlushAsync();

			#endregion

			#region 2.Update Frame's Delay & Disposal

			imageStream.Seek(0);
			var propertiesList = new BitmapPropertySet[frameCount];
			var decoder = await BitmapDecoder.CreateAsync(imageStream);
			for (int i = 0; i < frameCount; i++)
			{
				var frame = await decoder.GetFrameAsync((uint)i);
				var frameProperties = await frame.BitmapProperties.GetPropertiesAsync(_framePropertyKeys);
				if (frameProperties.TryGetValue(_grcelextDelayKey, out var grctlextDelayProperty))
				{
					var grctlextDelayTime = int.Parse(grctlextDelayProperty.Value.ToString());
					if (grctlextDelayTime == 0)
					{
						frameProperties[_grcelextDelayKey] = new BitmapTypedValue(frameDelays[i] / 10, PropertyType.UInt16);
					}
				}
				if (frameProperties.TryGetValue(_grcelextDisposalKey, out var _grcelextDisposalProperty))
				{
					var grctlextDisposalType = int.Parse(_grcelextDisposalProperty.Value.ToString());
					if (grctlextDisposalType == 0)
					{
						frameProperties[_grcelextDisposalKey] = new BitmapTypedValue(2, PropertyType.UInt8);
					}
				}
				propertiesList[i] = frameProperties;
			}

			#endregion

			#region 3.Reencode Whole GIF

			var reencodeStream = new InMemoryRandomAccessStream();
			encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, reencodeStream);
			await encoder.BitmapContainerProperties.SetPropertiesAsync(_appextProperties);
			for (var i = 0; i < frameCount; i++)
			{
				var animatedImage = animatedImages[i];
				var holder = animatedImage.Image;
				var framePixels = holder.PixelData;
				var frameProperties = propertiesList[i];
				await encoder.BitmapProperties.SetPropertiesAsync(frameProperties);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)holder.Width, (uint)holder.Height, 96, 96, framePixels);
				if (i != frameCount - 1)
				{
					await encoder.GoToNextFrameAsync();
				}
			}
			await encoder.FlushAsync();
			bitmapImage.SetSource(reencodeStream);

			#endregion

			return bitmapImage;
		}

		public static async Task<WriteableBitmap> ToBitmapImageAsync(this BitmapHolder holder, IMainThreadDispatcher mainThreadDispatcher)
        {
            if (holder == null || holder.PixelData == null)
                return null;

            WriteableBitmap writeableBitmap = null;

            await mainThreadDispatcher.PostAsync(async () =>
            {
                writeableBitmap = await holder.ToWriteableBitmap();
                writeableBitmap.Invalidate();
            }).ConfigureAwait(false);

            return writeableBitmap;
        }

        private static async Task<WriteableBitmap> ToWriteableBitmap(this BitmapHolder holder)
        {
            var writeableBitmap = new WriteableBitmap(holder.Width, holder.Height);

            using (var stream = writeableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(holder.PixelData, 0, holder.PixelData.Length);
            }

            return writeableBitmap;
        }

		public async static Task<BitmapImage> ToBitmapImageAsync(this Stream imageStream, IImageService imageService, double scale, Tuple<int, int> downscale, bool downscaleDipUnits, InterpolationMode mode, bool allowUpscale, ImageInformation imageInformation = null)
		{
			if (imageStream == null)
				return null;

			//NO NEED TO FREE RandomAccessStream, OTHERWISE Bitmap WILL NOT SHOW
			var image = imageStream.AsRandomAccessStream();
			if (image != null)
			{
				if (downscale != null && (downscale.Item1 > 0 || downscale.Item2 > 0))
				{
					var downscaledImage = await image.ResizeImage(imageService, scale, downscale.Item1, downscale.Item2, mode, downscaleDipUnits, allowUpscale, imageInformation).ConfigureAwait(false);
					if (downscaledImage != null)
					{
						BitmapDecoder decoder = await BitmapDecoder.CreateAsync(downscaledImage);
						downscaledImage.Seek(0);
						BitmapImage resizedBitmap = null;

						await imageService.Dispatcher.PostAsync(async () =>
						{
							resizedBitmap = new BitmapImage();
							await resizedBitmap.SetSourceAsync(downscaledImage);
						}).ConfigureAwait(false);

						return resizedBitmap;
					}
				}
				else
				{
					BitmapDecoder decoder = await BitmapDecoder.CreateAsync(image);
					image.Seek(0);
					BitmapImage bitmap = null;

					if (imageInformation != null)
					{
						imageInformation.SetCurrentSize((int)decoder.PixelWidth, (int)decoder.PixelHeight);
						imageInformation.SetOriginalSize((int)decoder.PixelWidth, (int)decoder.PixelHeight);
					}

					await imageService.Dispatcher.PostAsync(async () =>
					{
						bitmap = new BitmapImage();
						await bitmap.SetSourceAsync(image);
					}).ConfigureAwait(false);

					return bitmap;
				}
			}
			return null;
		}

		public async static Task<BitmapHolder> ToBitmapHolderAsync(this Stream imageStream, IImageService imageService, double scale, Tuple<int, int> downscale, bool downscaleDipUnits, InterpolationMode mode, bool allowUpscale, ImageInformation imageInformation = null)
		{
			if (imageStream == null)
				return null;

			//NO NEED TO FREE RandomAccessStream, OTHERWISE Bitmap WILL NOT SHOW
			var image = imageStream.AsRandomAccessStream();
			if (image != null)
			{
				if (downscale != null && (downscale.Item1 > 0 || downscale.Item2 > 0))
				{
					var downscaledImage = await image.ResizeImage(imageService, scale, downscale.Item1, downscale.Item2, mode, downscaleDipUnits, allowUpscale, imageInformation).ConfigureAwait(false);
					if (downscaledImage != null)
					{
						BitmapDecoder decoder = await BitmapDecoder.CreateAsync(downscaledImage);
						PixelDataProvider pixelDataProvider = await decoder.GetPixelDataAsync(
							BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, new BitmapTransform(),
							ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

						var bytes = pixelDataProvider.DetachPixelData();

						return new BitmapHolder(bytes, (int)decoder.PixelWidth, (int)decoder.PixelHeight);
					}
				}
				else
				{
					BitmapDecoder decoder = await BitmapDecoder.CreateAsync(image);
					PixelDataProvider pixelDataProvider = await decoder.GetPixelDataAsync(
						BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, new BitmapTransform(),
						ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

					if (imageInformation != null)
					{
						imageInformation.SetCurrentSize((int)decoder.PixelWidth, (int)decoder.PixelHeight);
						imageInformation.SetOriginalSize((int)decoder.PixelWidth, (int)decoder.PixelHeight);
					}

					var bytes = pixelDataProvider.DetachPixelData();

					return new BitmapHolder(bytes, (int)decoder.PixelWidth, (int)decoder.PixelHeight);
				}
			}
			return null;
		}

		private static unsafe void CopyPixels(byte[] data, int[] pixels)
        {
            int length = pixels.Length;

            fixed (byte* srcPtr = data)
            {
                fixed (int* dstPtr = pixels)
                {
                    for (var i = 0; i < length; i++)
                    {
                        dstPtr[i] = (srcPtr[i * 4 + 3] << 24)
                                  | (srcPtr[i * 4 + 2] << 16)
                                  | (srcPtr[i * 4 + 1] << 8)
                                  | srcPtr[i * 4 + 0];
                    }
                }
            }
        }

        public static async Task<IRandomAccessStream> ResizeImage(this IRandomAccessStream imageStream, IImageService imageService, double scale, int width, int height, InterpolationMode interpolationMode, bool useDipUnits, bool allowUpscale, ImageInformation imageInformation = null)
        {
            if (useDipUnits)
            {
                width = imageService.DpToPixels(width, scale);
                height = imageService.DpToPixels(height, scale);
            }

            IRandomAccessStream resizedStream = imageStream;
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            if ((height > 0 && decoder.PixelHeight > height) || (width > 0 && decoder.PixelWidth > width) || allowUpscale)
            {
				//NO NEED TO FREE RandomAccessStream, OTHERWISE Bitmap WILL NOT SHOW
				//using (imageStream)
				{
					resizedStream = new InMemoryRandomAccessStream();
                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    
                    double widthRatio = (double)width / decoder.PixelWidth;
                    double heightRatio = (double)height / decoder.PixelHeight;

                    double scaleRatio = Math.Min(widthRatio, heightRatio);

                    if (width == 0)
                        scaleRatio = heightRatio;

                    if (height == 0)
                        scaleRatio = widthRatio;

                    uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                    uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                    if (interpolationMode == InterpolationMode.None)
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Cubic;
                    else if (interpolationMode == InterpolationMode.Low)
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                    else if (interpolationMode == InterpolationMode.Medium)
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Cubic;
                    else if (interpolationMode == InterpolationMode.High)
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                    else
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Cubic;

                    encoder.BitmapTransform.ScaledHeight = aspectHeight;
                    encoder.BitmapTransform.ScaledWidth = aspectWidth;

                    if (imageInformation != null)
                    {
                        imageInformation.SetOriginalSize((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                        imageInformation.SetCurrentSize((int)aspectWidth, (int)aspectHeight);
                    }

                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                }
            }

            return resizedStream;
        }
    }
}
