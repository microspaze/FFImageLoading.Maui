using System;
using System.Collections.Generic;
using FFImageLoading;
using FFImageLoading.Config;
using FFImageLoading.Helpers;

namespace Sample
{
    public partial class BasicPage : ContentPage
	{
		private readonly IImageService _imageService = ServiceHelper.GetService<IImageService>();

		private static readonly string _token = Guid.NewGuid().ToString("N");
		private static HttpClient _httpClient;

		BasicPageModel viewModel = new BasicPageModel();

		public BasicPage()
        {
			_httpClient = new HttpClient(new AuthenticatedHttpImageClientHandler(GetToken));
			//Recommend to use IC to initialize Configuration instance, BTW new is also OK.
			//var ffconfig = new Configuration();
			var ffconfig = ServiceHelper.GetService<IConfiguration>();
			ffconfig.HttpClient = _httpClient;
			var imageService = ServiceHelper.GetService<IImageService>();
			imageService.Initialize(ffconfig);

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

		private string GetToken()
		{
			return _token;
		}
	}

	public class AuthenticatedHttpImageClientHandler : HttpClientHandler
	{
		private Func<string> _tokenFunc = null;

		public AuthenticatedHttpImageClientHandler(Func<string> tokenFunc)
		{
			_tokenFunc = tokenFunc;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var token = _tokenFunc.Invoke();
			if (!string.IsNullOrEmpty(token))
			{
				//request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);
				request.Headers.Add("m_t", token);
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
