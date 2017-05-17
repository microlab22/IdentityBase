﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceBase.Configuration;
using System;
using System.IO;
using ServiceBase.Extensions;

namespace IdentityBase.Public
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public static void Main(string[] args)
        {
            var contentRoot = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Configuration = ConfigurationSetup.Configure(contentRoot, environment, (confBuilder) =>
            {
                if ("Development".Equals(environment, StringComparison.OrdinalIgnoreCase))
                {
                    confBuilder.AddUserSecrets<Startup>();
                }

                confBuilder.AddCommandLine(args);
            });

            var configHost = Configuration.GetSection("Host");
            var configLogging = Configuration.GetSection("Logging");

            var hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(configHost["Urls"])
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(f => f.AddConsole(configLogging))
                .UseStartup<Startup>();
            
            if (configHost["UseIISIntegration"].ToBoolean())
            {
                hostBuilder = hostBuilder.UseIISIntegration();
            }

            hostBuilder.Build().Run();
        }
    }
}
