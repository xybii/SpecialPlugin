namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IOnApplicationShutdown
    {
        void OnApplicationShutdown(ApplicationShutdownContext context);
    }
}
