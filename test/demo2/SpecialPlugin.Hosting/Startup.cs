using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SpecialPlugin.AspNetCore;
using System;
using System.Collections.Generic;

namespace SpecialPlugin.Hosting
{
    public class Startup
    {
        public List<PluginModule> PluginModules = new List<PluginModule>();

        public void ConfigureServices(IServiceCollection services)
        {
            CreateGlobalLogger();

            var modules = Core.PluginExtensions.GetPluginSources<PluginModule>();

            services.AddMvc().ConfigureApplicationPartManager(apm =>
            {
                foreach (var module in modules)
                {
                    var pluginModele = Activator.CreateInstance(module) as PluginModule;

                    pluginModele.ConfigureServices(services);

                    PluginModules.Add(pluginModele);

                    foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(module.Assembly))
                    {
                        apm.ApplicationParts.Add(part);
                    }
                }

                foreach (var pluginRazor in Core.PluginExtensions.GetPluginRazors())
                {
                    apm.ApplicationParts.Add(pluginRazor);
                }
            });

            services.AddControllers();

            services.AddControllersWithViews().AddControllersAsServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            foreach (var pluginModele in PluginModules)
            {
                pluginModele.Configure(app);
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void CreateGlobalLogger()
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss:fff} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose() // 所有Sink的最小记录级别
                .Enrich.WithProperty("SourceContext", null) //加入属性SourceContext，也就运行时是调用Logger的具体类
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: outputTemplate)//输出到控制台
                .CreateLogger();
        }
    }
}
