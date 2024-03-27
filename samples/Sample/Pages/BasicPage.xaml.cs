using System;
using System.Collections.Generic;
using FFImageLoading;

namespace Sample
{
    public partial class BasicPage : ContentPage
    {
        BasicPageModel viewModel = new BasicPageModel();

		private readonly IImageService _imageService = FFImageLoading.Helpers.ServiceHelper.GetService<IImageService>();

		public BasicPage()
        {
            InitializeComponent();
			BindingContext = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		/// <summary>
		/// Clear All Image Cache
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void InvalidateCache(object sender, EventArgs e)
		{
			if (_imageService == ImageService.Instance)
			{
				_imageService.InvalidateMemoryCache();
				await _imageService.InvalidateDiskCacheAsync();
			}
		}
	}
}
