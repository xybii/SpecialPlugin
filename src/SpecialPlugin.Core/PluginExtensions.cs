using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin.Core
{
    public static class PluginExtensions
    {
        public static IEnumerable<Type> GetPluginSources<T>(string unitPackagesName = "UnitPackages", string searchPackagePattern = "*.dll") where T : class
        {
            return GetPluginSources<T>(unitPackagesName, searchPackagePattern, null);
        }

        public static IEnumerable<Type> GetPluginSources<T>(string unitPackagesName, string searchPackagePattern, SearchOption? searchOption) where T : class
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

            var root = new DirectoryInfo(path);

            DirectoryInfo[] dirs = root.GetDirectories();

            var assemblyDic = new Dictionary<Assembly, PluginLoadContext>();

            foreach (var item in dirs)
            {
                var files = item.GetFiles(searchPackagePattern).ToList();

                foreach (var file in files)
                {
                    using var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                    var context = new PluginLoadContext(file.FullName, searchOption);

                    try
                    {
                        var assembly = context.LoadFromStream(fs);

                        context.SetBaseAssembly(assembly);

                        assemblyDic.Add(assembly, context);

                        AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                    }
                    catch (BadImageFormatException)
                    {
                    }
                }
            }

            while (assemblyDic.Count > 0)
            {
                foreach (var item in assemblyDic.ToList())
                {
                    try
                    {
                        foreach (var type in item.Key.GetExportedTypes())
                        {
                            if (typeof(T).IsAssignableFrom(type))
                            {
                                moduleTypes.Add(type);

                                item.Value.ResgiterContext();
                            }
                        }

                        assemblyDic.Remove(item.Key);
                    }
                    catch (FileNotFoundException ex)
                    {
                        string assemblyName = ex.FileName;

                        if (assemblyDic.Values.SelectMany(o => o.Assemblies).Select(o => o.FullName).Contains(assemblyName))
                        {
                            continue;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            return moduleTypes;
        }

        public static IEnumerable<CompiledRazorAssemblyPart> GetPluginRazors(string unitPackagesName = "UnitPackages", string searchPackagePattern = "*.Views.dll")
        {
            List<CompiledRazorAssemblyPart> compiledRazorAssemblyParts = new List<CompiledRazorAssemblyPart>();

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
                    using var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                    var context = new PluginLoadContext(file.FullName);

                    var assembly = context.LoadFromStream(fs);

                    var viewAssemblyPart = new CompiledRazorAssemblyPart(assembly);

                    compiledRazorAssemblyParts.Add(viewAssemblyPart);
                }
            }

            return compiledRazorAssemblyParts;
        }
    }
}
