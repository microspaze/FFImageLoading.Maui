using FFImageLoading.Extensions;
using FFImageLoading.Helpers;
using System;
using Windows.UI;
using Microsoft.UI.Xaml.Media.Imaging;

namespace FFImageLoading.Work
{
    public class BitmapHolder : IBitmap
    {
		public BitmapHolder(BitmapImage bitmap)
		{
			BitmapImage = bitmap;
		}

        public BitmapHolder(WriteableBitmap bitmap)
        {
            WriteableBitmap = bitmap;
        }

		public BitmapHolder(byte[] pixels, int width, int height)
		{
			PixelData = pixels;
			Width = width;
			Height = height;
		}

		public bool HasBitmapImage => BitmapImage != null;

        public bool HasWriteableBitmap => WriteableBitmap != null;

		public BitmapImage BitmapImage { get; private set; }

		public WriteableBitmap WriteableBitmap { get; private set; }

		public int Height { get; private set; }

        public int Width { get; private set; }

		public byte[] PixelData { get; private set; }

		public int PixelCount { get { return PixelData == null ? 0 : (int)(PixelData.Length / 4); } }

        public void SetPixel(int x, int y, ColorHolder color)
        {
            int pixelPos = (y * Width + x);
            SetPixel(pixelPos, color);
        }

        public void SetPixel(int pos, ColorHolder color)
        {
            int bytePos = pos * 4;
            PixelData[bytePos] = color.B;
            PixelData[bytePos + 1] = color.G;
            PixelData[bytePos + 2] = color.R;
            PixelData[bytePos + 3] = color.A;
        }

        public ColorHolder GetPixel(int pos)
        {
            int bytePos = pos * 4;
            var b = PixelData[bytePos];
            var g = PixelData[bytePos + 1];
            var r = PixelData[bytePos + 2];
            var a = PixelData[bytePos + 3];

            return new ColorHolder(a, r, g, b);
        }

        public ColorHolder GetPixel(int x, int y)
        {
            int pixelPos = (y * Width + x);
            return GetPixel(pixelPos);
        }

		public void SetPixels(int[] pixels, int width, int height)
		{
			Width = width;
			Height = height;
			PixelData = new byte[width * height * 4];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var pos = y * width + x;
					var colorHolder = ColorHolder.ArgbToColorHolder(pixels[pos]);
					SetPixel(pos, colorHolder);
				}
			}
		}

		public void GetPixels(int[] pixels, int width, int height)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var pos = y * width + x;
					var colorHolder = GetPixel(x, y);
					pixels[pos] = ColorHolder.FromColorHolder(colorHolder);
				}
			}
		}

		public void FreePixels()
        {
            PixelData = null;
			BitmapImage = null;
            WriteableBitmap = null;
        }
	}

	public static class IBitmapExtensions
    {
        public static BitmapHolder ToNative(this IBitmap bitmap)
        {
            return (BitmapHolder)bitmap;
        }
    }
}
