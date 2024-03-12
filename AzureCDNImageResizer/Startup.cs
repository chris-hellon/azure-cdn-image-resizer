using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using AzureCDNImageResizer.Options;
using AzureCDNImageResizer.Services;
using AzureCDNImageResizer;
using AzureCDNImageResizer.Extensions;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureCDNImageResizer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddOptions<ImageResizerOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("ImageResizer").Bind(settings));

            builder.Services
                .AddOptions<ClientCacheOptions>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("ClientCache").Bind(settings));

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IImageResizerService, ImageResizerService>();

            builder.Services.FixConfiguration();
        }
    }
}
