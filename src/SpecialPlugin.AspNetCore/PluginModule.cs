using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Reflection;

namespace SpecialPlugin.AspNetCore
{
    public class PluginModule : IPluginModule
    {
        public ServiceConfigurationContext ServiceConfigurationContext { get; internal set; }

        public virtual void ConfigureServices(ServiceConfigurationContext context)
        {
        }

        public virtual void OnApplicationInitialization(ApplicationInitializationContext context)
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
    }
}
