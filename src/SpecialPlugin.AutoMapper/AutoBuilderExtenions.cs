using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin.AutoMapper
{
    public static class AutoBuilderExtenions
    {
        public static IEnumerable<Type> GetMappingTypes(this Assembly assembly, Type mappingInterface)
        {
            return assembly.GetTypes().Where(x => !x.IsAbstract && x.GetInterfaces().Contains(mappingInterface));
        }

        public static void AddAutoConfigurationsFromAssembly(this IMapperConfigurationExpression cfg, Assembly assembly)
        {
            var mappingTypes = assembly.GetMappingTypes(typeof(IAutoMappingConfiguration)).ToList();

            foreach (var config in mappingTypes.Select(Activator.CreateInstance).Cast<IAutoMappingConfiguration>())
            {
                config.Map(cfg);
            }
        }
    }
}
