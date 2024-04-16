
namespace Sample
{
    public partial class SimpleGifPage : ContentPage
    {
        SimpleGifPageModel viewModel;

		public SimpleGifPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new SimpleGifPageModel();
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

            viewModel.Reload();
		}

		private void OnChangeClicked(object sender, EventArgs e)
		{
			//Console.Write(cachedImageView.ImageView.Width);
			viewModel.Reload();
		}

		private void OnLoadSuccess(object sender, FFImageLoading.Maui.CachedImageEvents.SuccessEventArgs e)
		{
			Console.WriteLine("CachedImageView load success!");
        }
    }
}
