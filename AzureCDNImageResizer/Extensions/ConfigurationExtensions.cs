﻿using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AzureCDNImageResizer.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void FixConfiguration(this IServiceCollection services)
        {
            var providers = new List<IConfigurationProvider>();

            foreach (var descriptor in services.Where(descriptor => descriptor.ServiceType == typeof(IConfiguration)).ToList())
            {
                var existingConfiguration = descriptor.ImplementationInstance as IConfigurationRoot;
                if (existingConfiguration is null)
                {
                    continue;
                }
                providers.AddRange(existingConfiguration.Providers);
                services.Remove(descriptor);
            }

            var executioncontextoptions = services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;

            var currentDirectory = executioncontextoptions.AppDirectory;

            var config = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            providers.AddRange(config.Build().Providers);

            services.AddSingleton<IConfiguration>(new ConfigurationRoot(providers));
        }
    }
}

