using System;

namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IServiceProviderAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
}
