using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample
{

    public partial class BasicPageModel : ObservableObject
    {
        public void Reload()
        {
            // ImageUrl = Helpers.GetRandomImageUrl();
            ImageUrl = @"https://gastaticqn.gatime.cn/Landscape_3.jpg";
        }

		[ObservableProperty]
		string imageUrl;
    }
}
