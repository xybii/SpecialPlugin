using AutoMapper;

namespace SpecialPlugin.AutoMapper
{
    public interface IRegisterAutoMapper
    {
        void RegisterAutoMapperConfigure(IMapperConfigurationExpression mapExpression);
    }
}
