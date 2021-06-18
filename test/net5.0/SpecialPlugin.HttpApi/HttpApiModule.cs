using SpecialPlugin.AspNetCore;

namespace SpecialPlugin.HttpApi
{
    public class HttpApiModule : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            //services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);

            //services.AddSingleton(ActionDescriptorChangeProvider.Instance);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
        }
    }
}
