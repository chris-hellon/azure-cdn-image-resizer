using System;
using System.Linq;
using AzureCDNImageResizer.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AzureCDNImageResizer.Services;
using AzureCDNImageResizer.Options;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace AzureCDNImageResizer.Functions
{
	public class ImageResizeFunction
	{
        private readonly IImageResizerService imageResizerService;
        private readonly IOptions<ClientCacheOptions> clientCacheOptions;
        private readonly IConfiguration config;

        public ImageResizeFunction(IImageResizerService imageProxyService,
            IOptions<ClientCacheOptions> clientCacheOptions,
            IConfiguration configuration)
        {
            this.imageResizerService = imageProxyService;
            this.clientCacheOptions = clientCacheOptions;
            config = configuration;
        }

        [FunctionName("ResizeImage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ResizeImage/{*restOfPath}")] HttpRequest req, string restOfPath)
        {
            // check to see if we have a cached version and just leave if we do
            if (req.HttpContext.Request.GetTypedHeaders().IfModifiedSince.HasValue)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotModified);
            }

            try
            {
                var urlSplit = restOfPath.Split("/");
                var container = urlSplit[0];
                var url = "/";

                foreach (var splitUrl in urlSplit.Skip(1))
                {
                    url += splitUrl + "/";
                }

                url = url.TrimEnd('/');

                // we need at least the url
                if (string.IsNullOrEmpty(url))
                    return new BadRequestObjectResult("URL is required");

                // figure out the needed variables
                // var url = req.Query["url"].ToString();
                var size = req.Query.ContainsKey("size") ? req.Query["size"].ToString() : "";
                var width = req.Query.ContainsKey("w") ? req.Query["w"].ToString().ToInt() : 0;
                var height = req.Query.ContainsKey("h") ? req.Query["h"].ToString().ToInt() : 0;
                var output = req.Query.ContainsKey("output") ? req.Query["output"].ToString().Replace(".", "") : url.ToSuffix();
                var mode = req.Query.ContainsKey("mode") ? req.Query["mode"].ToString() : "";
                var videoOutputs = new List<string>()
                {
                    "mp4",
                    "mov",
                    "avi",
                    "wmv",
                    "webm"
                };
                var validOutputs = new List<string>() { "jpeg", "jpg", "gif", "png", "webp", "avif", "svg"};
                
                validOutputs.AddRange(videoOutputs);
                
                if (container == "")
                    return new OkResult();

                // validate the output
                if (!validOutputs.Contains(output))
                    output = url.ToSuffix();

                // figure out the actual size
                if (string.IsNullOrEmpty(size))
                    size = $"{width}x{height}";

                var isVideo = videoOutputs.Contains(output);
                
                // try to resize the image
                var imageStream = await this.imageResizerService.ResizeAsync(url, container, size, output, mode, isVideo);

                if (imageStream == null)
                    return new NotFoundResult();

                // choose the correct mime type
                var mimeType = output switch
                {
                    "jpeg" => "image/jpeg",
                    "jpg" => "image/jpeg",
                    "gif" => "image/gif",
                    "png" => "image/png",
                    "webp" => "image/webp",
                    "avif" => "image/webp",
                    "svg" => "image/svg",
                    "mp4" => "video/mp4",
                    "mov" => "video/mov",
                    "avi" => "video/avi",
                    "wmv" => "video/wmv",
                    "webm" => "video/webm",
                    _ => isVideo ? "video/mp4" : "image/jpeg"
                };

                // set cache 
                this.SetCacheHeaders(req.HttpContext.Response.GetTypedHeaders());

                // return the stream
                return new FileStreamResult(imageStream, mimeType);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }

        private void SetCacheHeaders(ResponseHeaders responseHeaders)
        {
            responseHeaders.CacheControl = new CacheControlHeaderValue { Public = true };
            responseHeaders.LastModified = new DateTimeOffset(new DateTime(1900, 1, 1));
            responseHeaders.Expires = new DateTimeOffset((DateTime.Now + this.clientCacheOptions.Value.MaxAge).ToUniversalTime());
        }

        private static string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".webp":
                    return "image/webp";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                default:
                    return "application/octet-stream";
            }
        }
    }
}

