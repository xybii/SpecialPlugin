using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore;

namespace SpecialPlugin.Plugin.A
{
    public class AModule : PluginModule
    {
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddScoped<AService>();
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            var b = app.ApplicationServices.GetRequiredService<AService>();
        }
    }
}
