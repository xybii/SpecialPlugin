using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin
{
    public class PluginExtensions
    {
        public static IHostBuilder CreateHostBuilder(string[] args, Type startupType, params string[] urls)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            ConfigureContainer(config,
                out var autoFacModule, out var startupFilter,
                out var serviceConfigActionList, out var containerConfigActionList,
                out var mapperConfigActionList, out var quartzConfigActionList);

            serviceConfigActionList.Add(services => services.AddSingleton<IStartupFilter>(startupFilter));

            serviceConfigActionList.Add(services => services.AddAutoMapper(cfg =>
            {
                mapperConfigActionList.ForAll(o => o.Invoke(cfg));
            }, typeof(AutoBuilderExtenions)));

            serviceConfigActionList.Add(services =>
            {
                services.AddQuartz(cfg =>
                {
                    quartzConfigActionList.ForAll(o => o.Invoke(cfg));

                    cfg.UseMicrosoftDependencyInjectionJobFactory();

                    cfg.SchedulerId = "Scheduler-Core";
                });

                services.AddQuartzServer(options =>
                {
                    options.WaitForJobsToComplete = true;
                });
            });

            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
                {
                    builder.RegisterModule(autoFacModule);
                }))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(startupType);

                    webBuilder.UseUrls(urls);

                    webBuilder.ConfigureServices(services =>
                    {
                        serviceConfigActionList.ForAll(o => o.Invoke(services));
                    });
                });
        }

        private static void ConfigureContainer(IConfigurationRoot configuration,
            out AutoFacModule autoFacModule,
            out PluginStartupFilter pluginStartupFilter,
            out List<Action<IServiceCollection>> serviceConfigActionList,
            out List<Action<ContainerBuilder>> containerConfigActionList,
            out List<Action<IMapperConfigurationExpression>> mapperConfigActionList,
            out List<Action<IServiceCollectionQuartzConfigurator>> quartzConfigActionList)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitPackages");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dics = root.GetDirectories();

            List<Assembly> assemblyList = new List<Assembly>();

            pluginStartupFilter = new PluginStartupFilter();

            serviceConfigActionList = new List<Action<IServiceCollection>>();

            containerConfigActionList = new List<Action<ContainerBuilder>>();

            mapperConfigActionList = new List<Action<IMapperConfigurationExpression>>();

            quartzConfigActionList = new List<Action<IServiceCollectionQuartzConfigurator>>();

            foreach (var item in dics)
            {
                var file = item.GetFiles("SpecialPlugin.*.dll").FirstOrDefault();

                if (file == null)
                {
                    continue;
                }

                using (var fs = new FileStream(file.FullName, FileMode.Open))
                {
                    var context = new PluginLoadContext(file.FullName);

                    var assembly = context.LoadFromStream(fs);

                    assemblyList.Add(assembly);

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
                        configuration
                    };

                    var startup = assembly.CreateInstance(startupType.FullName, true, BindingFlags.Default, null, startupParams, null, null) as PluginModule;

                    pluginStartupFilter.AddConfigureAction(o => startup.RegisterConfigure(o));

                    containerConfigActionList.Add(o => startup.RegisterAssemblyTypes(o));

                    serviceConfigActionList.Add(o => startup.RegisterConfigureServices(o));

                    mapperConfigActionList.Add(o => startup.RegisterAutoMapper(o));

                    quartzConfigActionList.Add(o => startup.RegisterQuartzJob(o));
                }
            }

            autoFacModule = new AutoFacModule(o =>
            {
                foreach (var assembly in assemblyList)
                {
                    o.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().InstancePerLifetimeScope();
                }
            });
        }
    }
}
