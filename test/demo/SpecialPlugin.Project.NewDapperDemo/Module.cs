using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.Project.NewDapperDemo.Dtos;
using SpecialPlugin.Project.NewDapperDemo.Models;
using SpecialPlugin.Web.Core;
using System.Linq;

namespace SpecialPlugin.Project.NewDapperDemo
{
    public class Module : StartupModule
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
