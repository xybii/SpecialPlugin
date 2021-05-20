using System;

namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IDependedTypesProvider
    {
        Type[] GetDependedTypes();
    }
}
