using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Project.OldDapperDemo;
using SpecialPlugin.Project.ReplaceController;

namespace SpecialPlugin.Project.ReplaceReplaceController
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Scoped<ITest, NNTest>());
        }
    }
}
