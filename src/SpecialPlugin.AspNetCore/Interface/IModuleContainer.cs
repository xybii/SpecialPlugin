using System.Collections.Generic;

namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IModuleContainer
    {
        IReadOnlyList<IModuleDescriptor> Modules { get; }
    }
}
