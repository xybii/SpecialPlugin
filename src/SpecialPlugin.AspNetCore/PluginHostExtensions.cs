﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    public static class PluginHostExtensions
    {
        public static IServiceCollection AddApplication<TStartupModule>(this IServiceCollection services, Action<ApplicationCreationOptions> optionsAction) where TStartupModule : IPluginModule
        {
            var options = new ApplicationCreationOptions();

            optionsAction?.Invoke(options);

            var context = new ServiceConfigurationContext(services);

            var applicationWithExternalServiceProvider = new ApplicationWithExternalServiceProvider(typeof(TStartupModule), services, optionsAction);

            services.AddSingleton(context);

            foreach (var item in applicationWithExternalServiceProvider.PluginModuleDescriptors)
            {
                if (item.Instance is PluginModule module)
                {
                    module.ServiceConfigurationContext = context;

                    module.ConfigureServices(context);

                    module.ServiceConfigurationContext = null;
                }
            }

            services.AddSingleton<IApplicationWithExternalServiceProvider>(applicationWithExternalServiceProvider);

            services.AddSingleton<ObjectAccessor<IServiceProvider>>();

            services.AddSingleton<ObjectAccessor<IApplicationBuilder>>();

            return services;
        }

        public static void InitializeApplication(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value = app;

            var application = app.ApplicationServices.GetRequiredService<IApplicationWithExternalServiceProvider>();

            application.Initialize(app.ApplicationServices);
        }

        public static IApplicationBuilder GetApplicationBuilder(this ApplicationInitializationContext context)
        {
            return context.ServiceProvider.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value;
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            var service = services
                .FirstOrDefault(o => o.ServiceType == typeof(HostBuilderContext))?
                .ImplementationInstance;

            var hostBuilderContext = service as HostBuilderContext;

            if (hostBuilderContext?.Configuration != null)
            {
                return hostBuilderContext.Configuration as IConfigurationRoot;
            }

            return services.FirstOrDefault(o => o.ServiceType == typeof(IConfiguration)).ImplementationType as IConfigurationRoot;
        }
    }
}
