using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    public class ApplicationWithExternalServiceProvider : ApplicationBase, IApplicationWithExternalServiceProvider
    {
        internal ApplicationWithExternalServiceProvider(Type startupModuleType, IServiceCollection services, Action<ApplicationCreationOptions> optionsAction) : base(startupModuleType, services, optionsAction)
        {
            services.AddSingleton<IApplicationWithExternalServiceProvider>(this);

            services.AddTransient<OnApplicationInitializationModuleLifecycleContributor>();

            services.AddTransient<OnPreApplicationInitializationModuleLifecycleContributor>();

            services.AddTransient<OnPostApplicationInitializationModuleLifecycleContributor>();
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            SetServiceProvider(serviceProvider);

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = new ApplicationInitializationContext(scope.ServiceProvider);

                var options = scope.ServiceProvider.GetRequiredService<IOptions<ModuleLifecycleOptions>>();

                var lifecycleContributors = options.Value.Contributors
                    .Select(serviceProvider.GetRequiredService)
                    .Cast<IModuleLifecycleContributor>()
                    .ToArray();

                foreach (var contributor in lifecycleContributors)
                {
                    foreach (var module in Modules)
                    {
                        try
                        {
                            contributor.Initialize(context, module.Instance);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"An error occurred during the initialize {contributor.GetType().FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                        }
                    }
                }
            }
        }
    }
}
