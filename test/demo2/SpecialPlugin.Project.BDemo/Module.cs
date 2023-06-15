using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SpecialPlugin.Project.ADemo;
using SpecialPlugin.Web.Core;

namespace SpecialPlugin.Project.BDemo
{
    public class Module : StartupModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Scoped<ITest, Test>());
        }
    }
}
