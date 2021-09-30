using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.AspNetCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace SpecialPlugin.Hosting
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var modules = Core.PluginExtensions.GetPluginSources<PluginModule>();

            services.AddApplication<HostModule>(o =>
            {
                o.PlugInSources.AddRange(modules);
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

            AddControllers(services);
        }

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
                endpoints.MapControllers();
            });
        }

        private void AddControllers(IServiceCollection services)
        {
            services.AddOptions();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                })
                .AddNewtonsoftJson(options => { options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss"; });

            services.AddControllersWithViews().AddControllersAsServices();
        }
    }
}
