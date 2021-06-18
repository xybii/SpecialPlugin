using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Reflection;

namespace SpecialPlugin.AspNetCore
{
    public class PluginModule : IPluginModule, IOnPreApplicationInitialization,
        IOnApplicationInitialization, IOnPostApplicationInitialization,
        IPreConfigureServices, IPostConfigureServices
    {
        public ServiceConfigurationContext ServiceConfigurationContext { get; internal set; }

        public virtual void PreConfigureServices(ServiceConfigurationContext context)
        {
        }

        public virtual void ConfigureServices(ServiceConfigurationContext context)
        {
        }

        public virtual void PostConfigureServices(ServiceConfigurationContext context)
        {
        }

        public virtual void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
        }

        public virtual void OnApplicationInitialization(ApplicationInitializationContext context)
        {
        }

        public virtual void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
        }

        public virtual void OnApplicationShutdown(ApplicationShutdownContext context)
        {
        }

        public static bool IsPluginModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(IPluginModule).GetTypeInfo().IsAssignableFrom(type);
        }

        internal static void CheckAbpModuleType(Type moduleType)
        {
            if (!IsPluginModule(moduleType))
            {
                throw new ArgumentException("Given type is not an Plugin module: " + moduleType.AssemblyQualifiedName);
            }
        }

        protected void Configure<TOptions>(Action<TOptions> configureOptions) where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure(configureOptions);
        }
    }
}
