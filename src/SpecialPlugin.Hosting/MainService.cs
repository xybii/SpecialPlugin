using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.AutoMapper;
using SpecialPlugin.Quartz;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SpecialPlugin.Hosting
{
    public class MainService
    {
        private readonly string[] _args;
        private IHost _webHost;

        public MainService(string[] args)
        {
            _args = args;
        }

        public void Start()
        {
            var isService = !(Debugger.IsAttached || _args.Contains("--console"));

            var builder = CreateHostBuilder(_args.Where(arg => arg != "--console").ToArray());

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;

                var pathToContentRoot = Path.GetDirectoryName(pathToExe);

                builder.UseContentRoot(pathToContentRoot);
            }

            _webHost = builder.Build();

            _webHost.RunAsync();
        }

        public void Stop()
        {
            _webHost.StopAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var modules = PluginExtensions.SelectPluginModule(options =>
            {
                options.ConfigurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).Build();
            });

            return PluginExtensions.CreateHostBuilder(args, modules,
                services =>
                {
                    services.AddPluginAutoMapper(modules, true);

                    services.AddPluginQuartz(modules);
                },
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.UseUrls("http://*:15555");
                });
        }
    }
}
