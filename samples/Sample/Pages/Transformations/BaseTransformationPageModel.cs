using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample
{
	
	public partial class BaseTransformationPageModel : ObservableObject
	{
		public BaseTransformationPageModel()
		{
		}

		[ObservableProperty]
		string imageUrl;

		[RelayCommand]
		public void LoadAnother()
			=> Reload(useGif: false);

		[RelayCommand]
		public void Reload(bool useGif = true)
		{
			ImageUrl = useGif ? "tenor.gif" : Helpers.GetRandomImageUrl();
		}
	}
}
