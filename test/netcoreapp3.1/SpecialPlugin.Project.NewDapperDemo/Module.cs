﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Project.NewDapperDemo.Dtos;
using SpecialPlugin.Project.NewDapperDemo.Models;
using System;
using System.IO;

namespace SpecialPlugin.Project.NewDapperDemo
{
    [DependsOn(typeof(JobModule))]
    public class Module : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            var configuration = services.GetConfiguration();

            services.Configure<NewDapperDemoOptions>(configuration.GetSection("NewDapperDemoOptions"));

            services.AddScoped<IJobService, JobService>();

            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<BookTag, BookTagDto>();
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitPackages", GetType().Namespace, $"wwwroot");

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(path),   //实际目录地址
                RequestPath = new PathString($"/Resource1"),
                EnableDirectoryBrowsing = true //开启目录浏览
            });

            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
            //}
        }
    }
}
