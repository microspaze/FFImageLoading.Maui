using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FFImageLoading.Transformations;
using FFImageLoading.Work;

namespace Sample
{
    public partial class CropTransformationPageModel : ObservableObject
    {
        double mDeltaPan = 0.02f;
        double mRatioZoom = 0.8f;

        [ObservableProperty]
        long originalSize;

        [ObservableProperty]
        long croppedSize;

		[ObservableProperty]
        string imageUrl;

        [ObservableProperty]
        List<ITransformation> transformations;

		public Action ReloadImageHandler { get; set; }

		public CropTransformationPageModel()
		{
			ImageUrl = "https://loremflickr.com/600/600/nature?filename=crop_transformation.jpg";

			CurrentZoomFactor = 1d;
			CurrentXOffset = 0d;
			CurrentYOffset = 0d;
		}

        public void ReloadImage()
        {
			Transformations = new List<ITransformation>()
			{
				new CropTransformation(CurrentZoomFactor, CurrentXOffset, CurrentYOffset, 1f, 1f)
			};

			ReloadImageHandler?.Invoke();
        }

        [ObservableProperty]
        double currentZoomFactor;

        [ObservableProperty]
        double currentXOffset;

        [ObservableProperty]
        double currentYOffset;

        public void OnPanUpdated(PanUpdatedEventArgs e)
        {
	        Console.WriteLine($"Pan status: {e.StatusType}");
            if (e.StatusType == GestureStatus.Running)
            {
                var xChange = Math.Abs(e.TotalX);
                var yChange = Math.Abs(e.TotalY);
                if (xChange >= yChange)
                {
	                CurrentXOffset += e.TotalX > 0 ? -mDeltaPan : mDeltaPan;
                }
                if (xChange <= yChange)
                {
	                CurrentYOffset += e.TotalY > 0 ? -mDeltaPan : mDeltaPan;
                }
                ReloadImage();
            }
        }

        public void OnPinchUpdated(PinchGestureUpdatedEventArgs e)
        {
	        Console.WriteLine($"Pinch status: {e.Status}");
            if (e.Status == GestureStatus.Running)
            {
                CurrentZoomFactor += (e.Scale - 1) * CurrentZoomFactor * mRatioZoom;
                CurrentZoomFactor = Math.Max(1, CurrentZoomFactor);
                if (CurrentZoomFactor == 1)
                {
	                CurrentXOffset = 0;
	                CurrentYOffset = 0;
                }
                ReloadImage();
            }
        }
    }
}
