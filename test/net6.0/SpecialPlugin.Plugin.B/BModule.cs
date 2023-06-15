using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore;

namespace SpecialPlugin.Plugin.B
{
    public class BModule : PluginModule
    {
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddScoped<BService>();
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
        }
    }
}
