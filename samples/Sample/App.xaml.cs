namespace Sample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			//Issue #3 Reproduce Sample
			//Set MainPage by Page with CachedImage may cause ImageService NRE
			//MainPage = new BasicPage();

			var m = new MenuPage();
			MainPage = new NavigationPage(m);
		}
	}
}
