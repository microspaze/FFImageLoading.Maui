using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFImageLoading
{
    public readonly struct ColorHolder
    {
        public ColorHolder(int a, int r, int g, int b)
        {
            A = Convert.ToByte(Math.Min(Math.Max(0, a), 255));

            if (a > 0)
            {
                R = Convert.ToByte(Math.Min(Math.Max(0, r), 255));
                G = Convert.ToByte(Math.Min(Math.Max(0, g), 255));
                B = Convert.ToByte(Math.Min(Math.Max(0, b), 255));
            }
            else
            {
                R = 0;
                G = 0;
                B = 0;
            }
        }

        public ColorHolder(int r, int g, int b)
        {
            A = 255;
            R = Convert.ToByte(Math.Min(Math.Max(0, r), 255));
            G = Convert.ToByte(Math.Min(Math.Max(0, g), 255));
            B = Convert.ToByte(Math.Min(Math.Max(0, b), 255));
        }

        public readonly byte A;

        public readonly byte R;

        public readonly byte G;

        public readonly byte B;

        public static readonly ColorHolder Transparent = new ColorHolder(0, 0, 0, 0);

		public static ColorHolder RgbaToColorHolder(int rgbaValue)
		{
			int r = (rgbaValue >> 24) & 0xFF;
			int g = (rgbaValue >> 16) & 0xFF;
			int b = (rgbaValue >> 8) & 0xFF;
			int a = rgbaValue & 0xFF;

			return new ColorHolder(a, r, g, b);
		}

		public static ColorHolder ArgbToColorHolder(int argbValue)
		{
			int a = (argbValue >> 24) & 0xFF;
			int r = (argbValue >> 16) & 0xFF;
			int g = (argbValue >> 8) & 0xFF;
			int b = argbValue & 0xFF;

			return new ColorHolder(a, r, g, b);
		}

		public static int FromColorHolder(ColorHolder colorHolder)
		{
			int r = colorHolder.R & 0xFF;
			int g = colorHolder.G & 0xFF;
			int b = colorHolder.B & 0xFF;
			int a = colorHolder.A & 0xFF;

			return (r << 24) + (g << 16) + (b << 8) + (a);
		}
	}
}
