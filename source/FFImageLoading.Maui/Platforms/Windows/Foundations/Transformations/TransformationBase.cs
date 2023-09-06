using System;
using FFImageLoading.Work;

namespace FFImageLoading.Transformations
{
    public abstract class TransformationBase : ITransformation
    {
        public abstract string Key { get; }

        public IBitmap Transform(IBitmap bitmapHolder, string path, Work.ImageSourceType sourceType, bool isPlaceholder, string key)
        {
            var nativeHolder = bitmapHolder.ToNative();
            return Transform(nativeHolder, path, sourceType, isPlaceholder, key);
        }

        protected virtual BitmapHolder Transform(BitmapHolder bitmapHolder, string path, Work.ImageSourceType sourceType, bool isPlaceholder, string key)
        {
            return bitmapHolder;
        }
    }
}
