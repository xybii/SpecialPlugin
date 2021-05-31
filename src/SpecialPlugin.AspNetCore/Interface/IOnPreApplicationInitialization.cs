namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IOnPreApplicationInitialization
    {
        void OnPreApplicationInitialization(ApplicationInitializationContext context);
    }
}
