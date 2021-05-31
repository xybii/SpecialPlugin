namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IPreConfigureServices
    {
        void PreConfigureServices(ServiceConfigurationContext context);
    }
}
