﻿using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;

namespace FFImageLoading.Transformations
{
    [Preserve(AllMembers = true)]
    public class GrayscaleTransformation : TransformationBase
    {
        public GrayscaleTransformation()
        {
        }

        public override string Key
        {
            get { return "GrayscaleTransformation"; }
        }

        protected override UIImage Transform(UIImage sourceBitmap, string path, Work.ImageSourceType sourceType, bool isPlaceholder, string key)
        {
            using (var effect = new CIPhotoEffectMono() { InputImage = sourceBitmap.CGImage })
            using (var output = effect.OutputImage)
            using (var context = CIContext.FromOptions(null))
            using (var cgimage = context.CreateCGImage(output, output.Extent))
            {
                return UIImage.FromImage(cgimage);
            }
        }
    }
}

