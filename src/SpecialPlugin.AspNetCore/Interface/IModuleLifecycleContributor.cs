namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IModuleLifecycleContributor
    {
        void Initialize(ApplicationInitializationContext context, IPluginModule module);

        void Shutdown(ApplicationShutdownContext context, IPluginModule module);
    }
}
