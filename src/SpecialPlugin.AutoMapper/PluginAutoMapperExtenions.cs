using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace SpecialPlugin.AutoMapper
{
    public static class PluginAutoMapperExtenions
    {
        public static IServiceCollection AddPluginAutoMapper(this IServiceCollection services,
            IList<PluginModule> pluginModules, bool addConfigurationsFromAssembly = false,
            Action<IMapperConfigurationExpression> configAction = null)
        {
            return services.AddAutoMapper(cfg =>
            {
                foreach (var pluginModule in pluginModules)
                {
                    if (pluginModule is IRegisterAutoMapper registerAutoMapper)
                    {
                        registerAutoMapper.RegisterAutoMapperConfigure(cfg);
                    }

                    if(addConfigurationsFromAssembly)
                    {
                        cfg.AddAutoConfigurationsFromAssembly(pluginModule.GetType().Assembly);
                    }
                }

                if (configAction != null)
                {
                    configAction.Invoke(cfg);
                }
            });
        }
    }
}
