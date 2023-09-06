using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FFImageLoading.Maui;


namespace FFImageLoading.Svg.Maui
{
	[Preserve(AllMembers = true)]
	/// <summary>
	/// SvgImageSourceConverter
	/// </summary>
	public class SvgImageSourceConverter : FFImageLoading.Maui.ImageSourceConverter, IValueConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);

		//public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		//	=> CoreConvertFrom(value);


		public new object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> CoreConvertFrom(value);

		object CoreConvertFrom(object value)
		{
			var text = value as string;
			if (string.IsNullOrWhiteSpace(text))
				return null;

			var xfSource = base.ConvertFrom(text) as ImageSource;

			if (text.IsSvgFileUrl() || text.IsSvgDataUrl())
			{
				return new SvgImageSource(xfSource, 0, 0, true);
			}

			return xfSource;
		}
	}
}
