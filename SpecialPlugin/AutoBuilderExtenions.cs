using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin
{
    public static class AutoBuilderExtenions
    {
        private static IEnumerable<Type> GetMappingTypes(this Assembly assembly, Type mappingInterface)
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

        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                cfg.AddAutoConfigurationsFromAssembly(assembly);
            }, typeof(AutoBuilderExtenions));
        }
    }
}
