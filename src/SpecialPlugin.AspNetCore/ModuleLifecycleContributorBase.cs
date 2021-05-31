using SpecialPlugin.AspNetCore.Interface;

namespace SpecialPlugin.AspNetCore
{
    public abstract class ModuleLifecycleContributorBase : IModuleLifecycleContributor
    {
        public virtual void Initialize(ApplicationInitializationContext context, IPluginModule module)
        {
        }
    }
}
