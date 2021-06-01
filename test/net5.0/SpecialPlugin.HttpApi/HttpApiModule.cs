using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore;
using System.Reflection;

namespace SpecialPlugin.HttpApi
{
    public class HttpApiModule : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);

            services.AddSingleton(ActionDescriptorChangeProvider.Instance);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var partManager = context.ServiceProvider.GetRequiredService<ApplicationPartManager>();

            var assembly = Assembly.GetExecutingAssembly();

            foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(assembly))
            {
                partManager.ApplicationParts.Add(part);
            }
        }
    }
}
