using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpecialPlugin
{
    public abstract class PluginModule
    {
        public IConfiguration Configuration { get; }

        public PluginModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void RegisterAssemblyTypes(ContainerBuilder containerBuilder) { }

        public virtual void RegisterConfigureServices(IServiceCollection services) { }

        public virtual void RegisterConfigure(IApplicationBuilder app) { }
    }
}
