using SpecialPlugin.AspNetCore.Interface;
using System;

namespace SpecialPlugin.AspNetCore
{
    public class ApplicationInitializationContext : IServiceProviderAccessor
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ApplicationInitializationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
