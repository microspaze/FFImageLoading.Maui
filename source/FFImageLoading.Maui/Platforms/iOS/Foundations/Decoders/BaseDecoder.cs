namespace FFImageLoading.Decoders
{
    public class BaseDecoder : GifDecoder
    {
		public BaseDecoder(IImageService imageService)
			:base(imageService)
		{

		}
    }
}
