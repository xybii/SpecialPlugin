using Quartz;

namespace SpecialPlugin.Quartz
{
    public interface IRegisterQuartz
    {
        void RegisterQuartzConfigure(IServiceCollectionQuartzConfigurator configurator);
    }
}