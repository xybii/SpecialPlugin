using SpecialPlugin.AspNetCore;
using SpecialPlugin.HttpApi;

namespace SpecialPlugin.Hosting
{
    [DependsOn(typeof(HttpApiModule))]
    public class HostModule : PluginModule
    {
    }
}
