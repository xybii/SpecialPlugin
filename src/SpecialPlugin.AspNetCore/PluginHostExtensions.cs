using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin.AspNetCore
{
    public static class PluginHostExtensions
    {
        public static IServiceCollection AddApplication<TStartupModule>(this IServiceCollection services, Action<ApplicationCreationOptions> optionsAction) where TStartupModule : IPluginModule
        {
            new ApplicationWithExternalServiceProvider(typeof(TStartupModule), services, optionsAction);

            services.AddSingleton<ObjectAccessor<IApplicationBuilder>>();

            return services;
        }

        public static void InitializeApplication(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value = app;

            var application = app.ApplicationServices.GetRequiredService<IApplicationWithExternalServiceProvider>();

            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                application.Shutdown();
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                application.Dispose();
            });

            application.Initialize(app.ApplicationServices);
        }

        public static void AddApplicationParts(this IApplicationBuilder app, Assembly assembly)
        {
            var partManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(assembly))
            {
                partManager.ApplicationParts.Add(part);
            }

            foreach (var part in new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(assembly))
            {
                partManager.ApplicationParts.Add(part);
            }
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
