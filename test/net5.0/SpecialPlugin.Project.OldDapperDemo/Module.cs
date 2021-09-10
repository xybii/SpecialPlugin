using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Quartz;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Project.OldDapperDemo.Dtos;
using SpecialPlugin.Project.OldDapperDemo.Models;
using System;
using System.IO;
using System.Reflection;

namespace SpecialPlugin.Project.OldDapperDemo
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            var configuration = services.GetConfiguration();

            services.Configure<OldDapperDemoOptions>(configuration.GetSection("OldDapperDemoOptions"));

            services.AddScoped<IJobService, JobService>();

            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<BookTag, BookTagDto>();
            }, typeof(Module));

            services.AddQuartz(cfg =>
            {
                cfg.UseMicrosoftDependencyInjectionScopedJobFactory();

                cfg.SchedulerId = "Scheduler-Core-OldDapperDemo";

                string key = "OldDapperDemoJob";

                var jobKey = new JobKey(key);

                cfg.AddJob<IJobService>(jobKey);

                cfg.AddTrigger(t => t
                    .WithIdentity(key)
                    .ForJob(jobKey)
                    .WithCronSchedule("0 0/1 * * * ?").WithDescription(key));
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitPackages", GetType().Namespace, $"wwwroot");

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(path),   //实际目录地址
                RequestPath = new PathString($"/Resource2"),
                EnableDirectoryBrowsing = true //开启目录浏览
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
            }
        }
    }
}
