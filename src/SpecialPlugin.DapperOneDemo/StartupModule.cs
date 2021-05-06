using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SpecialPlugin.AutoMapper;
using SpecialPlugin.DapperOneDemo.Dtos;
using SpecialPlugin.DapperOneDemo.Models;
using SpecialPlugin.Quartz;
using System;
using System.Reflection;

namespace SpecialPlugin.DapperOneDemo
{
    public class StartupModule : PluginModule, IRegisterAutoMapper, IRegisterQuartz
    {
        private readonly string _name;

        public StartupModule(IConfiguration configuration) : base(configuration)
        {
            _name = "DapperOneDemoJob";
        }

        public override void RegisterAssemblyTypes(ContainerBuilder containerBuilder)
        {
            Console.WriteLine($"{_name},RegisterAssemblyTypes");

            Assembly assembly = Assembly.GetExecutingAssembly();

            AutoFacModule autoFacModule = new AutoFacModule(o =>
            {
                o.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().InstancePerLifetimeScope();
            });

            containerBuilder.RegisterModule(autoFacModule);
        }

        public void RegisterAutoMapperConfigure(IMapperConfigurationExpression mapExpression)
        {
            Console.WriteLine($"{_name},RegisterAutoMapper");

            mapExpression.CreateMap<BookTag, BookTagDto>();
        }

        public override void RegisterConfigure(IApplicationBuilder app)
        {
            Console.WriteLine($"{_name},RegisterConfigure");

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
            }
        }

        public override void RegisterConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"{_name},RegisterConfigureServices");

            services.Configure<DapperOneDemoOptions>(Configuration.GetSection("DapperOneDemoOptions"));
        }

        public void RegisterQuartzConfigure(IServiceCollectionQuartzConfigurator configurator)
        {
            Console.WriteLine($"{_name},RegisterQuartzJob");

            string key = "DapperOneDemoJob";

            var jobKey = new JobKey(key);

            configurator.AddJob<IJobService>(jobKey);

            configurator.AddTrigger(t => t
                .WithIdentity(key)
                .ForJob(jobKey)
                .WithCronSchedule("0 0/1 * * * ?").WithDescription(key)
            );
        }
    }
}
