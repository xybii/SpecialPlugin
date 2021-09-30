using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Project.ADemo;

namespace SpecialPlugin.Project.BDemo
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Scoped<ITest, Test>());
        }
    }
}
