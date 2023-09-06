using Android.Graphics;
using FFImageLoading.Work;

namespace FFImageLoading.Transformations
{
    public abstract class TransformationBase : ITransformation
    {
        public abstract string Key { get; }

        public IBitmap Transform(IBitmap bitmapHolder, string path, Work.ImageSourceType sourceType, bool isPlaceholder, string key)
        {
            var sourceBitmap = bitmapHolder.ToNative();
            return new BitmapHolder(Transform(sourceBitmap, path, sourceType, isPlaceholder, key));
        }

        protected virtual Bitmap Transform(Bitmap sourceBitmap, string path, Work.ImageSourceType sourceType, bool isPlaceholder, string key)
        {
            return sourceBitmap;
        }
    }
}

