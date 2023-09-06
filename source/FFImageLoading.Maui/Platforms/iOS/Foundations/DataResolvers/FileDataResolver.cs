using System;
using FFImageLoading.Work;
using System.IO;
using FFImageLoading.IO;
using System.Threading.Tasks;
using FFImageLoading.Helpers;
using System.Threading;
using Foundation;
using Microsoft.Maui.Storage;

namespace FFImageLoading.DataResolvers
{
	public class FileDataResolver : IDataResolver
	{
		public virtual Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
		{
			string file = null;

			var scale = (int)parameters.Scale;
			if (scale > 1)
			{
				var filename = Path.GetFileNameWithoutExtension(identifier);
				var extension = Path.GetExtension(identifier);
				const string pattern = "{0}@{1}x{2}";

				while (scale > 1)
				{
					token.ThrowIfCancellationRequested();

					var tmpFile = string.Format(pattern, filename, scale, extension);
					if (FileStore.Exists(tmpFile))
					{
						file = tmpFile;
						break;
					}
					scale--;
				}
			}

			if (string.IsNullOrEmpty(file) && FileStore.Exists(identifier))
			{
				file = identifier;
			}

			// Let's check the bundle and bundle resource paths too
			foreach (var bu in NSBundle.AllBundles)
			{
				var path = Path.Combine(bu.ResourcePath, identifier);

				if (File.Exists(path))
				{
					file = path;
					break;
				}

				path = Path.Combine(bu.BundlePath, identifier);

				if (File.Exists(path))
				{
					file = path;
					break;
				}
			}


			token.ThrowIfCancellationRequested();

			if (!string.IsNullOrEmpty(file))
			{
				var stream = FileStore.GetInputStream(file, true);
				var imageInformation = new ImageInformation();
				imageInformation.SetPath(identifier);
				imageInformation.SetFilePath(file);

				var result = (LoadingResult)(int)parameters.SourceType;

				if (parameters.LoadingPlaceholderPath == identifier)
					result = (LoadingResult)(int)parameters.LoadingPlaceholderSourceType;
				else if (parameters.ErrorPlaceholderPath == identifier)
					result = (LoadingResult)(int)parameters.ErrorPlaceholderSourceType;

				return Task.FromResult(new DataResolverResult(
					stream, result, imageInformation));
			}

			throw new FileNotFoundException(identifier);
		}
	}
}

