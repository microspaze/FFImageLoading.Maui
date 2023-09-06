using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample
{
    public partial class SimpleWebpPageModel : ObservableObject
    {
		public void Reload()
		{
			ImageUrl = "https://gastaticqn.gatime.cn/1.sm.webp";
		}

		[ObservableProperty]
		string imageUrl;
    }
}
