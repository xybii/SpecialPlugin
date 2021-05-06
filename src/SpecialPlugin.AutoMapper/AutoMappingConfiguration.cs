using AutoMapper;
using System;

namespace SpecialPlugin.AutoMapper
{
    public interface IAutoMappingConfiguration
    {
        void Map(IMapperConfigurationExpression cfg);
    }

    public abstract class AutoMappingConfiguration : IAutoMappingConfiguration
    {
        public abstract void Map(IMapperConfigurationExpression cfg);
    }

    public interface IAutoMappingConfiguration<T> : IAutoMappingConfiguration where T : class
    {
    }

    public abstract class AutoMappingConfiguration<T> : IAutoMappingConfiguration<T> where T : class
    {
        public abstract void Map(IMapperConfigurationExpression cfg);
    }
}
