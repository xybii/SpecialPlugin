using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    public static class PlugInSourceExtensions
    {
        public static Type[] GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(type => ModuleHelper.FindAllModuleTypes(type))
                .Distinct()
                .ToArray();
        }
    }
}
