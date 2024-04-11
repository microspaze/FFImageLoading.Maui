
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample
{
    public partial class SimpleGifPageModel : ObservableObject
    {
		//TODO: loading3.gif takes long time for parsing in GifHelper.cs
		private static int _imageUrlIndex = 0;
		private static readonly string[] _imageUrls = ["tenor.gif", "cat.gif", "duck.gif", "letter3d.gif", "loading2.gif", "loading3.gif"];

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
