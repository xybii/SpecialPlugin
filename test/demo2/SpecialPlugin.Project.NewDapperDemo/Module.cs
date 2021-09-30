using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Project.NewDapperDemo.Dtos;
using SpecialPlugin.Project.NewDapperDemo.Models;
using System;
using System.IO;
using System.Linq;

namespace SpecialPlugin.Project.NewDapperDemo
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            var configuration = GetConfiguration(services);

            services.Configure<NewDapperDemoOptions>(configuration.GetSection("NewDapperDemoOptions"));

            services.AddScoped<IJobService, JobService>();

            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<BookTag, BookTagDto>();
            }, typeof(Module));
        }

        public override void Configure(IApplicationBuilder app)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitPackages", GetType().Namespace, $"wwwroot");

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(path),   //实际目录地址
                RequestPath = new PathString($"/Resource1"),
                EnableDirectoryBrowsing = true //开启目录浏览
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IJobService>().Execute().GetAwaiter().GetResult();
            }
        }

        public static IConfiguration GetConfiguration(IServiceCollection services)
        {
            var service = services
                .FirstOrDefault(o => o.ServiceType == typeof(HostBuilderContext))?
                .ImplementationInstance;

            var hostBuilderContext = service as HostBuilderContext;

            if (hostBuilderContext?.Configuration != null)
            {
                return hostBuilderContext.Configuration as IConfigurationRoot;
            }

            return services.FirstOrDefault(o => o.ServiceType == typeof(IConfiguration)).ImplementationType as IConfigurationRoot;
        }
    }
}
