namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IOnApplicationInitialization
    {
        void OnApplicationInitialization(ApplicationInitializationContext context);
    }
}
