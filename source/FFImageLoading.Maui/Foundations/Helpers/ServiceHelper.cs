using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFImageLoading.Helpers
{
	public static class ServiceHelper
	{
		private static IServiceProvider _serviceProvider = null;

		public static T GetService<T>() where T : class
		{
			var service = default(T);
			var serviceProvider = GetServiceProvider();
			if (serviceProvider != null)
			{
				service = serviceProvider.GetService<T>();
			}

			return service;
		}

		public static T GetRequiredService<T>() where T : class
		{
			var service = default(T);
			var serviceProvider = GetServiceProvider();
			if (serviceProvider != null)
			{
				service = serviceProvider.GetRequiredService<T>();
			}

			return service;
		}

		private static IServiceProvider GetServiceProvider()
		{
			if (_serviceProvider == null)
			{
				_serviceProvider = Application.Current.Handler?.MauiContext?.Services;
				_serviceProvider ??= Application.Current.Windows.FirstOrDefault()?.Handler?.MauiContext.Services;
			}

			return _serviceProvider;
		}
	}
}
