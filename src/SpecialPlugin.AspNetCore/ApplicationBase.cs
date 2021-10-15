using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    public class ApplicationBase : IApplication
    {
        public Type StartupModuleType { get; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IServiceCollection Services { get; }

        public IReadOnlyList<IModuleDescriptor> Modules { get; }

        internal ApplicationBase(
            Type startupModuleType,
            IServiceCollection services,
            Action<ApplicationCreationOptions> optionsAction)
        {
            StartupModuleType = startupModuleType;
            Services = services;

            services.AddSingleton<ObjectAccessor<IServiceProvider>>();
            services.AddSingleton<IApplication>(this);
            services.AddSingleton<IModuleContainer>(this);

            services.Configure<ModuleLifecycleOptions>(options =>
            {
                options.Contributors.Add<OnPreApplicationInitializationModuleLifecycleContributor>();
                options.Contributors.Add<OnApplicationInitializationModuleLifecycleContributor>();
                options.Contributors.Add<OnPostApplicationInitializationModuleLifecycleContributor>();
                options.Contributors.Add<OnApplicationShutdownModuleLifecycleContributor>();
            });

            var options = new ApplicationCreationOptions();

            optionsAction?.Invoke(options);

            Modules = LoadModules(services, startupModuleType, options.PlugInSources);

            ConfigureServices();
        }

        public virtual void Shutdown()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var modules = Modules.Reverse().ToList();

                var options = scope.ServiceProvider.GetRequiredService<IOptions<ModuleLifecycleOptions>>();

                var lifecycleContributors = options.Value.Contributors
                    .Select(ServiceProvider.GetRequiredService)
                    .Cast<IModuleLifecycleContributor>()
                    .ToArray();

                var context = new ApplicationShutdownContext(scope.ServiceProvider);

                foreach (var contributor in lifecycleContributors)
                {
                    foreach (var module in modules)
                    {
                        try
                        {
                            contributor.Shutdown(context, module.Instance);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"An error occurred during the shutdown {contributor.GetType().FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                        }
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            //TODO: Shutdown if not done before?
        }

        protected virtual void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = ServiceProvider;
        }

        protected virtual void InitializeModules()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = new ApplicationInitializationContext(scope.ServiceProvider);

                var options = scope.ServiceProvider.GetRequiredService<IOptions<ModuleLifecycleOptions>>();

                var lifecycleContributors = options.Value.Contributors
                    .Select(ServiceProvider.GetRequiredService)
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

        protected List<IModuleDescriptor> LoadModules(IServiceCollection services, Type startupModuleType, PlugInSourceList plugInSources)
        {
            var modules = GetDescriptors(services, startupModuleType, plugInSources);

            modules = ModuleHelper.SortByDependency(modules, startupModuleType);

            return modules;
        }

        private List<IModuleDescriptor> GetDescriptors(
            IServiceCollection services,
            Type startupModuleType,
            PlugInSourceList plugInSources)
        {
            var modules = new List<ModuleDescriptor>();

            //All modules starting from the startup module
            foreach (var moduleType in ModuleHelper.FindAllModuleTypes(startupModuleType))
            {
                modules.Add(CreateModuleDescriptor(services, moduleType));
            }

            //Plugin modules
            foreach (var moduleType in plugInSources.GetAllModules())
            {
                if (modules.Any(m => m.Type == moduleType))
                {
                    continue;
                }

                modules.Add(CreateModuleDescriptor(services, moduleType));
            }

            ModuleHelper.SetDependencies(modules);

            return modules.Cast<IModuleDescriptor>().ToList();
        }

        protected virtual ModuleDescriptor CreateModuleDescriptor(IServiceCollection services, Type moduleType)
        {
            return new ModuleDescriptor(moduleType, CreateAndRegisterModule(services, moduleType));
        }

        protected virtual IPluginModule CreateAndRegisterModule(IServiceCollection services, Type moduleType)
        {
            var module = (IPluginModule)Activator.CreateInstance(moduleType);
            services.AddSingleton(moduleType, module);
            return module;
        }

        protected Type[] GetAllModules(PlugInSourceList plugInSources)
        {
            return plugInSources
                .SelectMany(pluginSource => GetModulesWithAllDependencies(pluginSource))
                .Distinct()
                .ToArray();
        }

        protected Type[] GetModulesWithAllDependencies(IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(type => ModuleHelper.FindAllModuleTypes(type))
                .Distinct()
                .ToArray();
        }

        protected void ConfigureServices()
        {
            var context = new ServiceConfigurationContext(Services);

            Services.AddSingleton(context);

            foreach (var module in Modules)
            {
                if (module.Instance is PluginModule abpModule)
                {
                    abpModule.ServiceConfigurationContext = context;
                }
            }

            //PreConfigureServices
            foreach (var module in Modules.Where(m => m.Instance is IPreConfigureServices))
            {
                try
                {
                    ((IPreConfigureServices)module.Instance).PreConfigureServices(context);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred during {nameof(IPreConfigureServices.PreConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
                }
            }

            //ConfigureServices
            foreach (var module in Modules)
            {
                try
                {
                    module.Instance.ConfigureServices(context);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred during {nameof(IPluginModule.ConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
                }
            }

            //PostConfigureServices
            foreach (var module in Modules.Where(m => m.Instance is IPostConfigureServices))
            {
                try
                {
                    ((IPostConfigureServices)module.Instance).PostConfigureServices(context);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred during {nameof(IPostConfigureServices.PostConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
                }
            }

            foreach (var module in Modules)
            {
                if (module.Instance is PluginModule pluginModule)
                {
                    pluginModule.ServiceConfigurationContext = null;
                }
            }
        }
    }
}
