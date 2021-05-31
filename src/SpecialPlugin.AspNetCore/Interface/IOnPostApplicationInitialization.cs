namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IOnPostApplicationInitialization
    {
        void OnPostApplicationInitialization(ApplicationInitializationContext context);
    }
}
