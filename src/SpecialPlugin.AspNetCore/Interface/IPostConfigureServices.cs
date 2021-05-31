namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IPostConfigureServices
    {
        void PostConfigureServices(ServiceConfigurationContext context);
    }
}
