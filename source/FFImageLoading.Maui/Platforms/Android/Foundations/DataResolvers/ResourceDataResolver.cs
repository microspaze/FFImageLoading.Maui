using FFImageLoading.Work;
using System.Threading.Tasks;
using Android.Content;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using AndroidX.Core.Content;
using Android.Graphics.Drawables;

namespace FFImageLoading.DataResolvers
{
    public class ResourceDataResolver : IDataResolver
    {
        static ConcurrentDictionary<string, int> _resourceIdentifiersCache = new ConcurrentDictionary<string, int>();

        public virtual Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
        {
            // Resource name is always without extension
            string resourceName = Path.GetFileNameWithoutExtension(identifier);

            if (!_resourceIdentifiersCache.TryGetValue(resourceName, out var resourceId))
            {
                token.ThrowIfCancellationRequested();
                resourceId = Context.Resources.GetIdentifier(resourceName.ToLowerInvariant(), "drawable", Context.PackageName);
                _resourceIdentifiersCache.TryAdd(resourceName.ToLowerInvariant(), resourceId);
            }

            if (resourceId == 0)
                throw new FileNotFoundException(identifier);

            token.ThrowIfCancellationRequested();
			Stream stream = Context.Resources.OpenRawResource(resourceId);

			if (stream == null)
				throw new FileNotFoundException(identifier);

			var density = 0;
			var bitmapWidth = 0;
			var bitmapHeight = 0;
			var drawable = ContextCompat.GetDrawable(Context, resourceId);
			if (drawable != null && drawable is BitmapDrawable bitmapDrawable)
			{
				var bitmap = bitmapDrawable.Bitmap;
				if (bitmap != null)
				{
					density = bitmap.Density;
					bitmapWidth = bitmap.Width;
					bitmapHeight = bitmap.Height;
				}
			}

			var imageInformation = new ImageInformation();
            imageInformation.SetPath(identifier);
            imageInformation.SetFilePath(identifier);
			imageInformation.SetDensity(density);
			imageInformation.SetOriginalSize(bitmapWidth, bitmapHeight);

			return Task.FromResult(new DataResolverResult(
				stream, LoadingResult.CompiledResource, imageInformation));
        }

        protected Context Context => new ContextWrapper(Android.App.Application.Context);
    }
}

