using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    internal class ApplicationWithExternalServiceProvider : ApplicationBase, IApplicationWithExternalServiceProvider
    {
        internal ApplicationWithExternalServiceProvider(Type startupModuleType, IServiceCollection services, Action<ApplicationCreationOptions> optionsAction) : base(startupModuleType, services, optionsAction)
        {
            services.AddSingleton<IApplicationWithExternalServiceProvider>(this);

            services.AddTransient<OnApplicationInitializationModuleLifecycleContributor>();

            services.AddTransient<OnPreApplicationInitializationModuleLifecycleContributor>();

            services.AddTransient<OnPostApplicationInitializationModuleLifecycleContributor>();

            services.AddTransient<OnApplicationShutdownModuleLifecycleContributor>();
        }

        void IApplicationWithExternalServiceProvider.SetServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider != null)
            {
                if (ServiceProvider != serviceProvider)
                {
                    throw new Exception("Service provider was already set before to another service provider instance.");
                }

                return;
            }

            SetServiceProvider(serviceProvider);
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            SetServiceProvider(serviceProvider);

            InitializeModules();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (ServiceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
    }
}
