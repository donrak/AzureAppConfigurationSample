using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Identity;

namespace DemoMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var settings = config.Build();
                        config.AddAzureAppConfiguration(options =>
                        {
                            var credentials = new ManagedIdentityCredential("7ef18e0c-e2c2-49ce-8caf-7098438cc473");
                            options.Connect(new Uri(settings["AppConfig:Endpoint"]), credentials)
                                .ConfigureRefresh(refresh =>
                                {
                                    refresh.Register("TestApp:Settings:Sentinel", refreshAll: true)
                                        .SetCacheExpiration(new TimeSpan(0, 0, 10));
                                })
                                .UseFeatureFlags()
                                .ConfigureKeyVault(kv =>
                                {
                                    kv.SetCredential(credentials);
                                });
                        });
                    })
                .UseStartup<Startup>());
    }
}
