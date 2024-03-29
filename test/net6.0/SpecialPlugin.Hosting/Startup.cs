using Autofac;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Core;
using SpecialPlugin.Web.Core;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace SpecialPlugin.Hosting
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var modules = PluginExtensions.GetPluginSources<PluginModule>();

            services.AddApplication<HostModule>(o =>
            {
                o.PlugInSources.AddTypes(modules.ToArray());
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

                foreach (var pluginRazor in PluginExtensions.GetPluginRazors())
                {
                    apm.ApplicationParts.Add(pluginRazor);
                }
            });

            AddControllers(services);
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
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutoFacModule(null));
        }

        private void AddControllers(IServiceCollection services)
        {
            // Add services to the collection. Don't build or return
            // any IServiceProvider or the ConfigureContainer method
            // won't get called.
            // https://docs.autofac.org/en/latest/integration/aspnetcore.html
            // https://github.com/aspnet/AspNetCore.Docs/issues/11441
            services.AddOptions();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                });
        }
    }
}
