using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.Web.Core;
using System;
using System.Collections.Generic;

namespace SpecialPlugin.Hosting
{
    public class Startup
    {
        public List<StartupModule> PluginModules = new List<StartupModule>();

        public void ConfigureServices(IServiceCollection services)
        {
            var modules = Core.PluginExtensions.GetPluginSources<StartupModule>();

            foreach(var module in modules)
            {
                var pluginModele = Activator.CreateInstance(module) as StartupModule;

                pluginModele.ConfigureServices(services);

                PluginModules.Add(pluginModele);
            }
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
        }
    }
}
