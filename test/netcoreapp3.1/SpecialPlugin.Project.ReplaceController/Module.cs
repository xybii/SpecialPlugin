using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore;
using System.Reflection;

namespace SpecialPlugin.Project.ReplaceController
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddMvc().ConfigureApplicationPartManager(apm =>
            {
                var assembly = Assembly.GetExecutingAssembly();

                foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(assembly))
                {
                    apm.ApplicationParts.Add(part);
                }

                foreach (var part in new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(assembly))
                {
                    apm.ApplicationParts.Add(part);
                }
            });
        }
    }
}
