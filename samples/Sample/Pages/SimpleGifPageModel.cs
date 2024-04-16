
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample
{
    public partial class SimpleGifPageModel : ObservableObject
    {
		//TODO: loading3.gif takes long time for parsing in GifHelper.cs
		private static int _imageUrlIndex = 0;
		private static readonly string[] _imageUrls = ["tenor.gif", "no_avatar.png", "cat.gif", "lake.webp", "duck.gif", "letter3d.gif", "loading2.gif", "loading3.gif"];

		public ICommand SuccessCommand { get; set; }

		public SimpleGifPageModel()
		{
			SuccessCommand = new RelayCommand(() =>
			{
				Console.WriteLine("CachedImageView command success!");
			});
		}


		public void Reload()
        {
			ImageUrl = _imageUrls[_imageUrlIndex];
			_imageUrlIndex++;
			if (_imageUrlIndex >= _imageUrls.Length)
			{
				_imageUrlIndex = 0;
			}
		}

        [ObservableProperty]
        string imageUrl;
	}
}
