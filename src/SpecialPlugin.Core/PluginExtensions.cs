using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpecialPlugin.Core
{
    public static class PluginExtensions
    {
        public static IEnumerable<Type> GetPluginSources<T>(string unitPackagesName = "UnitPackages", string searchPackagePattern = "*.dll") where T : class
        {
            List<Type> moduleTypes = new List<Type>();

            if (string.IsNullOrEmpty(unitPackagesName))
            {
                throw new ArgumentException("UnitPackagesName parameter cannot be empty");
            }

            if (string.IsNullOrEmpty(searchPackagePattern))
            {
                throw new ArgumentException("SearchPackagePattern parameter cannot be empty");
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, unitPackagesName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dics = root.GetDirectories();

            foreach (var item in dics)
            {
                var files = item.GetFiles(searchPackagePattern).ToList();

                foreach (var file in files)
                {
                    using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        var context = new PluginLoadContext(file.FullName);

                        var assembly = context.LoadFromStream(fs);

                        var types = assembly.GetTypes().Where(type =>
                        type.IsClass &&
                        !type.IsAbstract &&
                        !type.IsGenericType &&
                        typeof(T).IsAssignableFrom(type)).ToList();

                        moduleTypes.AddRange(types);
                    }
                }
            }

            return moduleTypes;
        }

        public static IEnumerable<CompiledRazorAssemblyPart> GetPluginRazors(string unitPackagesName = "UnitPackages", string searchPackagePattern = "*.Views.dll")
        {
            List<CompiledRazorAssemblyPart> compiledRazorAssemblyParts = new List<CompiledRazorAssemblyPart>();

            if(string.IsNullOrEmpty(unitPackagesName))
            {
                throw new ArgumentException("UnitPackagesName parameter cannot be empty");
            }

            if (string.IsNullOrEmpty(searchPackagePattern))
            {
                throw new ArgumentException("SearchPackagePattern parameter cannot be empty");
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, unitPackagesName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dics = root.GetDirectories();

            foreach (var item in dics)
            {
                var files = item.GetFiles(searchPackagePattern).ToList();

                foreach (var file in files)
                {
                    using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        var context = new PluginLoadContext(file.FullName);

                        var assembly = context.LoadFromStream(fs);

                        var viewAssemblyPart = new CompiledRazorAssemblyPart(assembly);

                        compiledRazorAssemblyParts.Add(viewAssemblyPart);
                    }
                }
            }

            return compiledRazorAssemblyParts;
        }
    }
}
