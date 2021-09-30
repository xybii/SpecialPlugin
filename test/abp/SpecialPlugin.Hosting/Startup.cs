using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;

namespace SpecialPlugin.Hosting
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var modules = Core.PluginExtensions.GetPluginSources<AbpModule>().ToArray();

            services.AddApplication<HostModule>(o =>
            {
                o.PlugInSources.AddTypes(modules);
            });

            services.AddMvc().ConfigureApplicationPartManager(apm =>
            {
                foreach (var type in modules)
                {
                    foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(type.Assembly))
                    {
                        apm.ApplicationParts.Add(part);
                    }
                }

                foreach (var pluginRazor in Core.PluginExtensions.GetPluginRazors())
                {
                    apm.ApplicationParts.Add(pluginRazor);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.InitializeApplication();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
