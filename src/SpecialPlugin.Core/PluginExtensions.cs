using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin.Core
{
    public static class PluginExtensions
    {
        public static List<System.Type> GetPlugInSources<Type>(string unitPackagesName = "UnitPackages", string searchPackagePattern = "SpecialPlugin.*.dll")
        {
            List<System.Type> moduleTypes = new List<System.Type>();

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
                    using (var fs = new FileStream(file.FullName, FileMode.Open))
                    {
                        var context = new PluginLoadContext(file.FullName);

                        var assembly = context.LoadFromStream(fs);

                        var types = assembly.GetTypes().Where(type =>
                        type.IsClass &&
                        !type.IsAbstract &&
                        !type.IsGenericType &&
                        typeof(Type).GetTypeInfo().IsAssignableFrom(type));

                        moduleTypes.AddRange(types);
                    }
                }
            }

            return moduleTypes;
        }
    }
}
