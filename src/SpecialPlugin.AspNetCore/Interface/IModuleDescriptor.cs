using System;
using System.Collections.Generic;

namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IModuleDescriptor
    {
        Type Type { get; }

        IPluginModule Instance { get; }

        IReadOnlyList<IModuleDescriptor> Dependencies { get; }
    }
}
