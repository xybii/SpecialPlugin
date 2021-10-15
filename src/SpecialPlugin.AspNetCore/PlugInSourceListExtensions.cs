using System;

namespace SpecialPlugin.AspNetCore
{
    public static class PlugInSourceListExtensions
    {
        public static void AddTypes(this PlugInSourceList list, params Type[] moduleTypes)
        {
            list.Add(new TypePlugInSource(moduleTypes));
        }
    }
}
