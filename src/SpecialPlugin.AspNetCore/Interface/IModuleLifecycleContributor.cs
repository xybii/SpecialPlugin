namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IModuleLifecycleContributor
    {
        void Initialize(ApplicationInitializationContext context, IPluginModule module);
    }
}
