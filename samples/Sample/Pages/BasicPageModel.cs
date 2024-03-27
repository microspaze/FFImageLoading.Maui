using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample
{

    public partial class BasicPageModel : ObservableObject
    {
        public BasicPageModel()
        {
            // ImageUrl = Helpers.GetRandomImageUrl();
            ImageUrl = @"https://gastaticqn.gatime.cn/Landscape_3.jpg";

			var items = new List<BasicImageItem>();
			for (int i = 0; i < 50; i++)
			{
				items.Add(new BasicImageItem());
			}

			Items = [..items];
        }

		[ObservableProperty]
		string imageUrl;

		[ObservableProperty]
		List<BasicImageItem> items = [];
	}

	public partial class BasicImageItem : ObservableObject
	{
		[ObservableProperty]
		string image = Helpers.GetRandomImageUrl();

		[ObservableProperty]
		decimal price = 10.5M;
	}
}
