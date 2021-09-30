using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore;

namespace SpecialPlugin.Project.ADemo
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITest, Test>();
        }
    }
}
