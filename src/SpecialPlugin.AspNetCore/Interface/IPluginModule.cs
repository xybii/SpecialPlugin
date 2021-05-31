namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IPluginModule
    {
        void ConfigureServices(ServiceConfigurationContext context);
    }
}
