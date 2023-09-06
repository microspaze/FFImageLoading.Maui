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

		public IDataResolver GetResolver(string identifier, Work.ImageSourceType sourceType, TaskParameter parameters)
        {
            switch (sourceType)
            {
                case Work.ImageSourceType.ApplicationBundle:
                case Work.ImageSourceType.CompiledResource:
				case Work.ImageSourceType.Filepath:
					return new FallbackDataResolver(
						new ResourceDataResolver(),
						new BundleDataResolver(),
						new FileDataResolver());
                case Work.ImageSourceType.Url:
                    if (!string.IsNullOrWhiteSpace(identifier) && identifier.IsDataUrl())
                        return new DataUrlResolver();
                    return new UrlDataResolver(configuration, downloadCache);
                case Work.ImageSourceType.Stream:
                    return new StreamDataResolver();
                case Work.ImageSourceType.EmbeddedResource:
                    return new EmbeddedResourceResolver();
                default:
                    throw new NotSupportedException("Unknown type of ImageSource");
            }
        }
    }
}
