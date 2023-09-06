using FFImageLoading.Work;

namespace FFImageLoading.DataResolvers
{
	public class FallbackDataResolver : IDataResolver
	{
		public FallbackDataResolver(params IDataResolver[] resolvers)
		{
			DataResolvers = resolvers;
		}

		public readonly IDataResolver[] DataResolvers;

		public async Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
		{
			var exceptions = new List<Exception>();

			foreach (var resolver in DataResolvers)
			{
				try
				{
					var result = await resolver.Resolve(identifier, parameters, token).ConfigureAwait(false);
					return result;
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			}

			throw new AggregateException(exceptions.ToArray());
		}
	}
}
