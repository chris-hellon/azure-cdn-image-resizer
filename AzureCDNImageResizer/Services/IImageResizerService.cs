using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureCDNImageResizer.Services
{
    public interface IImageResizerService
    {
        Task<Stream> ResizeAsync(string url, string containerkey, string size, string output, string mode, bool isVideo = false);
    }
}

