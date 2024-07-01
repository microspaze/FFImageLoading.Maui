#if !ANDROID && !WINDOWS && !IOS && !TIZEN && !MACCATALYST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Work;

namespace FFImageLoading.Transformations
{
	public abstract class TransformationBase : ITransformation
	{
		public abstract string Key { get; }

		public IBitmap Transform(IBitmap bitmapHolder, string path, Work.ImageSourceType sourceType, bool isPlaceholder, string key)
		{
			return bitmapHolder;
		}
	}

	public class BlurredTransformation : TransformationBase
	{
		public BlurredTransformation()
		{
			Radius = 20d;
		}

		public BlurredTransformation(double radius)
		{
			Radius = radius;
		}

		public double Radius { get; set; }

		public override string Key
		{
			get { return string.Format("BlurredTransformation,radius={0}", Radius); }
		}
	}
	public class CircleTransformation : TransformationBase
	{
		public CircleTransformation() : this(0d, null)
		{
		}

		public CircleTransformation(double borderSize, string borderHexColor)
		{
			BorderSize = borderSize;
			BorderHexColor = borderHexColor;
		}

		public double BorderSize { get; set; }
		public string BorderHexColor { get; set; }

		public override string Key
		{
			get { return string.Format("CircleTransformation,borderSize={0},borderHexColor={1}", BorderSize, BorderHexColor); }
		}
	}
	public class ColorFillTransformation : TransformationBase
	{
		public ColorFillTransformation() : this("#000000")
		{
		}

		public ColorFillTransformation(string hexColor)
		{
			HexColor = hexColor;
		}

		public string HexColor { get; set; }

		public override string Key => string.Format("ColorFillTransformation,hexColor={0}", HexColor);
	}
	public class ColorSpaceTransformation : TransformationBase
	{
		float[][] _rgbawMatrix;

		public ColorSpaceTransformation() : this(FFColorMatrix.InvertColorMatrix)
		{
		}

		public ColorSpaceTransformation(float[][] rgbawMatrix)
		{
			if (rgbawMatrix.Length != 5 || rgbawMatrix.Any(v => v.Length != 5))
				throw new ArgumentException("Wrong size of RGBAW color matrix");

			RGBAWMatrix = rgbawMatrix;
		}

		public float[][] RGBAWMatrix
		{
			get
			{
				return _rgbawMatrix;
			}

			set
			{
				if (value.Length != 5 || value.Any(v => v.Length != 5))
					throw new ArgumentException("Wrong size of RGBAW color matrix");

				_rgbawMatrix = value;
			}
		}

		public override string Key
		{
			get
			{
				return string.Format("ColorSpaceTransformation,rgbawMatrix={0}",
					string.Join(",", _rgbawMatrix.Select(x => string.Join(",", x)).ToArray()));
			}
		}
	}
	public class CornersTransformation : TransformationBase
	{
		public CornersTransformation() : this(20d, CornerTransformType.TopRightRounded)
		{
		}

		public CornersTransformation(double cornersSize, CornerTransformType cornersTransformType)
			: this(cornersSize, cornersSize, cornersSize, cornersSize, cornersTransformType, 1d, 1d)
		{
		}

		public CornersTransformation(double topLeftCornerSize, double topRightCornerSize, double bottomLeftCornerSize, double bottomRightCornerSize,
			CornerTransformType cornersTransformType)
			: this(topLeftCornerSize, topRightCornerSize, bottomLeftCornerSize, bottomRightCornerSize, cornersTransformType, 1d, 1d)
		{
		}

		public CornersTransformation(double cornersSize, CornerTransformType cornersTransformType, double cropWidthRatio, double cropHeightRatio)
			: this(cornersSize, cornersSize, cornersSize, cornersSize, cornersTransformType, cropWidthRatio, cropHeightRatio)
		{
		}

		public CornersTransformation(double topLeftCornerSize, double topRightCornerSize, double bottomLeftCornerSize, double bottomRightCornerSize,
			CornerTransformType cornersTransformType, double cropWidthRatio, double cropHeightRatio)
		{
			TopLeftCornerSize = topLeftCornerSize;
			TopRightCornerSize = topRightCornerSize;
			BottomLeftCornerSize = bottomLeftCornerSize;
			BottomRightCornerSize = bottomRightCornerSize;
			CornersTransformType = cornersTransformType;
			CropWidthRatio = cropWidthRatio;
			CropHeightRatio = cropHeightRatio;
		}

		public double TopLeftCornerSize { get; set; }
		public double TopRightCornerSize { get; set; }
		public double BottomLeftCornerSize { get; set; }
		public double BottomRightCornerSize { get; set; }
		public double CropWidthRatio { get; set; }
		public double CropHeightRatio { get; set; }
		public CornerTransformType CornersTransformType { get; set; }

		public override string Key
		{
			get
			{
				return string.Format("CornersTransformation,cornersSizes={0},{1},{2},{3},cornersTransformType={4},cropWidthRatio={5},cropHeightRatio={6},",
					TopLeftCornerSize, TopRightCornerSize, BottomRightCornerSize, BottomLeftCornerSize, CornersTransformType, CropWidthRatio, CropHeightRatio);
			}
		}
	}
	public class CropTransformation : TransformationBase
	{
		public CropTransformation() : this(1d, 0d, 0d)
		{
		}

		public CropTransformation(double zoomFactor, double xOffset, double yOffset) : this(zoomFactor, xOffset, yOffset, 1f, 1f)
		{
		}

		public CropTransformation(double zoomFactor, double xOffset, double yOffset, double cropWidthRatio, double cropHeightRatio)
		{
			ZoomFactor = zoomFactor;
			XOffset = xOffset;
			YOffset = yOffset;
			CropWidthRatio = cropWidthRatio;
			CropHeightRatio = cropHeightRatio;

			if (ZoomFactor < 1f)
				ZoomFactor = 1f;
		}

		public double ZoomFactor { get; set; }
		public double XOffset { get; set; }
		public double YOffset { get; set; }
		public double CropWidthRatio { get; set; }
		public double CropHeightRatio { get; set; }

		public override string Key
		{
			get
			{
				return string.Format("CropTransformation,zoomFactor={0},xOffset={1},yOffset={2},cropWidthRatio={3},cropHeightRatio={4}",
					ZoomFactor, XOffset, YOffset, CropWidthRatio, CropHeightRatio);
			}
		}
	}
	public class FlipTransformation : TransformationBase
	{
		public FlipTransformation() : this(FlipType.Horizontal)
		{
		}

		public FlipTransformation(FlipType flipType)
		{
			FlipType = flipType;
		}

		public override string Key
		{
			get { return string.Format("FlipTransformation,Type={0}", FlipType); }
		}

		public FlipType FlipType { get; set; }
	}
	public class GrayscaleTransformation : TransformationBase
	{
		public GrayscaleTransformation()
		{
		}

		public override string Key
		{
			get { return "GrayscaleTransformation"; }
		}
	}
	public class RotateTransformation : TransformationBase
	{
		public RotateTransformation() : this(30d)
		{
		}

		public RotateTransformation(double degrees) : this(degrees, false, false)
		{
		}

		public RotateTransformation(double degrees, bool ccw) : this(degrees, ccw, false)
		{
		}

		public RotateTransformation(double degrees, bool ccw, bool resize)
		{
			Degrees = degrees;
			CCW = ccw;
			Resize = resize;
		}

		public double Degrees { get; set; }
		public bool CCW { get; set; }
		public bool Resize { get; set; }

		public override string Key
		{
			get { return string.Format("RotateTransformation,degrees={0},ccw={1},resize={2}", Degrees, CCW, Resize); }
		}
	}
	public class RoundedTransformation : TransformationBase
	{
		public RoundedTransformation() : this(30d)
		{
		}

		public RoundedTransformation(double radius) : this(radius, 1d, 1d)
		{
		}

		public RoundedTransformation(double radius, double cropWidthRatio, double cropHeightRatio) : this(radius, cropWidthRatio, cropHeightRatio, 0d, null)
		{
		}

		public RoundedTransformation(double radius, double cropWidthRatio, double cropHeightRatio, double borderSize, string borderHexColor)
		{
			Radius = radius;
			CropWidthRatio = cropWidthRatio;
			CropHeightRatio = cropHeightRatio;
			BorderSize = borderSize;
			BorderHexColor = borderHexColor;
		}

		public double Radius { get; set; }
		public double CropWidthRatio { get; set; }
		public double CropHeightRatio { get; set; }
		public double BorderSize { get; set; }
		public string BorderHexColor { get; set; }

		public override string Key
		{
			get
			{
				return string.Format("RoundedTransformation,radius={0},cropWidthRatio={1},cropHeightRatio={2},borderSize={3},borderHexColor={4}",
				Radius, CropWidthRatio, CropHeightRatio, BorderSize, BorderHexColor);
			}
		}
	}
	public class SepiaTransformation : TransformationBase
	{
		public SepiaTransformation()
		{
		}

		public override string Key
		{
			get { return "SepiaTransformation"; }
		}
	}
	public class TintTransformation : TransformationBase
	{

		public TintTransformation() : this(0, 165, 0, 128)
		{
		}

		public TintTransformation(int r, int g, int b, int a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public TintTransformation(string hexColor)
		{
			HexColor = hexColor;
		}

		string _hexColor;
		public string HexColor
		{
			get
			{
				return _hexColor;
			}

			set
			{
				_hexColor = value;
				var color = Color.FromArgb(value);
				A = (int)color.Alpha;
				R = (int)color.Red;
				G = (int)color.Green;
				B = (int)color.Blue;
			}
		}

		public bool EnableSolidColor { get; set; }

		public int R { get; set; }

		public int G { get; set; }

		public int B { get; set; }

		public int A { get; set; }

		public override string Key
		{
			get
			{
				return string.Format("TintTransformation,R={0},G={1},B={2},A={3},HexColor={4},EnableSolidColor={5}",
									 R, G, B, A, HexColor, EnableSolidColor);
			}
		}
	}
}
#endif
