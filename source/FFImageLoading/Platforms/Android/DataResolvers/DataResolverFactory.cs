using System;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using FFImageLoading.Work;

namespace FFImageLoading.DataResolvers
{


	public class DataResolverFactory : IDataResolverFactory
    {
		public DataResolverFactory(IConfiguration configuration, IDownloadCache downloadCache)
		{
			this.configuration = configuration;
			this.downloadCache = downloadCache;
		}

		readonly IConfiguration configuration;
		readonly IDownloadCache downloadCache;

		public IDataResolver GetResolver(string identifier, Work.ImageSource source, TaskParameter parameters)
        {
            switch (source)
            {
                case Work.ImageSource.ApplicationBundle:
                case Work.ImageSource.CompiledResource:
				case Work.ImageSource.Filepath:
					return new FallbackDataResolver(
						new ResourceDataResolver(),
						new BundleDataResolver(),
						new FileDataResolver());
                case Work.ImageSource.Url:
                    if (!string.IsNullOrWhiteSpace(identifier) && identifier.IsDataUrl())
                        return new DataUrlResolver();
                    return new UrlDataResolver(configuration, downloadCache);
                case Work.ImageSource.Stream:
                    return new StreamDataResolver();
                case Work.ImageSource.EmbeddedResource:
                    return new EmbeddedResourceResolver();
                default:
                    throw new NotSupportedException("Unknown type of ImageSource");
            }
        }
    }
}
