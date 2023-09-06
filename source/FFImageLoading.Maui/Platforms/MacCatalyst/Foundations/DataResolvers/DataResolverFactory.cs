using System;
using FFImageLoading.Work;
using FFImageLoading.Config;
using FFImageLoading.Cache;
using FFImageLoading.Helpers;

namespace FFImageLoading.DataResolvers
{
    public class DataResolverFactory : IDataResolverFactory
    {
		public DataResolverFactory(IConfiguration configuration, IDownloadCache downloadCache, IMainThreadDispatcher mainThreadDispatcher)
		{
			this.mainThreadDispatcher= mainThreadDispatcher;
			this.configuration = configuration;
			this.downloadCache = downloadCache;
		}

		readonly IMainThreadDispatcher mainThreadDispatcher;
		readonly IConfiguration configuration;
		readonly IDownloadCache downloadCache;

		public virtual IDataResolver GetResolver(string identifier, Work.ImageSourceType sourceType, TaskParameter parameters)
        {
            switch (sourceType)
            {
                case Work.ImageSourceType.Filepath:
                    return new FileDataResolver();
				case Work.ImageSourceType.ApplicationBundle:
				case Work.ImageSourceType.CompiledResource:
                    return new BundleDataResolver(mainThreadDispatcher);
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

