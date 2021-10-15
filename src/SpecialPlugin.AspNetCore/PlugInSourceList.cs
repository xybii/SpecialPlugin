using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    public class PlugInSourceList : List<IPlugInSource>
    {
        internal Type[] GetAllModules()
        {
            return this
                .SelectMany(pluginSource => pluginSource.GetModulesWithAllDependencies())
                .Distinct()
                .ToArray();
        }
    }
}
