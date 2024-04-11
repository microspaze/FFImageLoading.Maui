
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
			viewModel.Reload();
		}
    }
}
