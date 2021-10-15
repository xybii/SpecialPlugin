using System;

namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IPlugInSource
    {
        Type[] GetModules();
    }
}
