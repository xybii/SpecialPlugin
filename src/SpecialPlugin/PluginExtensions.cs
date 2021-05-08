using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin
{
    public class PluginExtensions
    {
        public static List<PluginModule> SelectPluginModule(Action<SelectPluginModuleOptions> configAction)
        {
            SelectPluginModuleOptions selectPluginModuleOptions = new SelectPluginModuleOptions();

            configAction.Invoke(selectPluginModuleOptions);

            List<PluginModule> pluginModules = new List<PluginModule>();

            selectPluginModuleOptions.ConfigurationRoot ??= new ConfigurationBuilder().Build();

            if (string.IsNullOrEmpty(selectPluginModuleOptions.UnitPackagesName))
            {
                throw new ArgumentException("UnitPackagesName parameter cannot be empty");
            }

            if (string.IsNullOrEmpty(selectPluginModuleOptions.SearchPackagePattern))
            {
                throw new ArgumentException("SearchPackagePattern parameter cannot be empty");
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectPluginModuleOptions.UnitPackagesName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dics = root.GetDirectories();

            foreach (var item in dics)
            {
                var files = item.GetFiles("SpecialPlugin.*.dll").ToList();

                foreach (var file in files)
                {
                    using (var fs = new FileStream(file.FullName, FileMode.Open))
                    {
                        var context = new PluginLoadContext(file.FullName);

                        var assembly = context.LoadFromStream(fs);

                        var types = assembly.GetTypes()
                        .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
                        .Where(type => type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract);

                        var startupType = types.Where(type => type.BaseType == typeof(PluginModule)).FirstOrDefault();

                        if (startupType == null)
                        {
                            continue;
                        }

                        var startupParams = new object[1]
                        {
                            selectPluginModuleOptions.ConfigurationRoot
                        };

                        var startup = assembly.CreateInstance(startupType.FullName, true, BindingFlags.Default, null, startupParams, null, null) as PluginModule;

                        pluginModules.Add(startup);
                    }
                }
            }

            return pluginModules;
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IList<PluginModule> pluginModules)
        {
            var assemblyList = new List<Assembly>();

            var pluginStartupFilter = new PluginStartupFilter();

            var serviceConfigActionList = new List<Action<IServiceCollection>>();

            var containerConfigActionList = new List<Action<ContainerBuilder>>();

            if (pluginModules != null)
            {
                foreach (var pluginModule in pluginModules)
                {
                    pluginStartupFilter.AddConfigureAction(o => pluginModule.RegisterConfigure(o));

                    containerConfigActionList.Add(o => pluginModule.RegisterAssemblyTypes(o));

                    serviceConfigActionList.Add(o => pluginModule.RegisterConfigureServices(o));

                    assemblyList.Add(Assembly.GetAssembly(pluginModule.GetType()));
                }
            }

            var autoFacModule = new AutoFacModule(o =>
            {
                foreach (var assembly in assemblyList)
                {
                    o.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().InstancePerLifetimeScope();
                }
            });

            serviceConfigActionList.Add(services => services.AddSingleton<IStartupFilter>(pluginStartupFilter));

            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
                {
                    builder.RegisterModule(autoFacModule);
                }))
                .ConfigureServices((_, services) =>
                {
                    foreach (var item in serviceConfigActionList)
                    {
                        item.Invoke(services);
                    }
                });
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Action<SelectPluginModuleOptions> configAction)
        {
            return CreateHostBuilder(args, SelectPluginModule(configAction));
        }
    }
}