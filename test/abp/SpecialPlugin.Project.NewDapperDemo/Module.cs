using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.Project.NewDapperDemo.Dtos;
using SpecialPlugin.Project.NewDapperDemo.Models;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SpecialPlugin.Project.NewDapperDemo
{
    [DependsOn(typeof(JobModule))]
    public class Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            var configuration = services.GetConfiguration();

            services.Configure<NewDapperDemoOptions>(configuration.GetSection("NewDapperDemoOptions"));

            services.AddScoped<IJobService, JobService>();

            services.AddMvc().ConfigureApplicationPartManager(apm =>
            {
                var assembly = Assembly.GetExecutingAssembly();

                foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(assembly))
                {
                    apm.ApplicationParts.Add(part);
                }

                foreach (var part in new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(assembly))
                {
                    apm.ApplicationParts.Add(part);
                }
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<BookTag, BookTagDto>();
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
            }
        }
    }
}
