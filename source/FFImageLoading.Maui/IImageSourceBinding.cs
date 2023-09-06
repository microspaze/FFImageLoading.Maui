using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FFImageLoading.Work;

namespace FFImageLoading.Maui
{
    [Preserve(AllMembers = true)]
    public interface IImageSourceBinding
    {
        Work.ImageSourceType ImageSourceType { get; }

        string Path { get; }

        Func<CancellationToken, Task<Stream>> Stream { get; }
    }
}
