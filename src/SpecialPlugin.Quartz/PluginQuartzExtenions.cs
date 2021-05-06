using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;

namespace SpecialPlugin.Quartz
{
    public static class PluginQuartzExtenions
    {
        public static IServiceCollection AddPluginQuartz(this IServiceCollection services,
            IList<PluginModule> pluginModules,
            Action<IServiceCollectionQuartzConfigurator> configAction = null)
        {
            services.AddQuartz(cfg =>
            {
                if (configAction != null)
                {
                    configAction.Invoke(cfg);
                }

                foreach (var pluginModule in pluginModules)
                {
                    var registerAutoMapper = pluginModule as IRegisterQuartz;

                    if (registerAutoMapper != null)
                    {
                        registerAutoMapper.RegisterQuartzConfigure(cfg);
                    }
                }

                cfg.UseMicrosoftDependencyInjectionJobFactory();

                cfg.SchedulerId = "Scheduler-Core";
            });

            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            return services;
        }
    }
}
