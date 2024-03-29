using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SpecialPlugin.Core
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly SearchOption? _searchOption;
        private Assembly _baseAssembly;
        
        internal static List<PluginLoadContext> PluginLoadContexts = new List<PluginLoadContext>();

        public PluginLoadContext(string pluginPath, SearchOption? searchOption = null)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
            _searchOption = searchOption;

            Resolving += PluginLoadContext_Resolving;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            IntPtr intPtr = IntPtr.Zero;

            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                intPtr = LoadUnmanagedDllFromPath(libraryPath);
            }

            if (intPtr == IntPtr.Zero && _searchOption.HasValue)
            {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{unmanagedDllName}.dll", _searchOption.Value);

                if (files.Length > 0)
                {
                    intPtr = LoadUnmanagedDllFromPath(files[0]);
                }
            }

            return intPtr;
        }

        public void SetBaseAssembly(Assembly assembly)
        {
            _baseAssembly = assembly;
        }

        public void ResgiterContext()
        {
            PluginLoadContexts.Add(this);

            Default.Resolving += (context, assemblyName) =>
            {
                var assemblyFullName = PluginLoadContexts?
                .Select(o => o._baseAssembly)?
                .Where(p => p != null && p.FullName == assemblyName.FullName)?
                .FirstOrDefault();

                if (assemblyFullName != null)
                {
                    return Assemblies.FirstOrDefault(p => p.FullName == assemblyName.FullName);
                }

                return null;
            };
        }

        private Assembly PluginLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            Assembly assembly = null;

            string assemblyPath = _resolver.ResolveAssemblyToPath(arg2);

            if (assemblyPath != null)
            {
                assembly = LoadFromAssemblyPath(assemblyPath);
            }

            return assembly ?? Load(_baseAssembly, arg2);
        }

        private Assembly Load(Assembly baseAssembly, AssemblyName assemblyName)
        {
            if (baseAssembly == null || assemblyName == null)
            {
                return null;
            }

            Assembly assembly = null;

            var assemblyNames = baseAssembly.GetReferencedAssemblies().Select(o => o.FullName);

            var contexts = PluginLoadContexts?
                .Where(o => o._baseAssembly != baseAssembly)
                .Where(o => o.Assemblies.Any(p => assemblyNames.Contains(p.FullName)))
                .ToList();

            if (contexts?.Count > 0)
            {
                foreach (var context in contexts)
                {
                    var assemblyN = context._baseAssembly.GetReferencedAssemblies().FirstOrDefault(o => o.FullName == assemblyName.FullName);

                    if (assemblyN != null)
                    {
                        assembly = context?.LoadFromAssemblyName(assemblyN);
                    }
                }
            }

            if (assembly == null && _searchOption.HasValue)
            {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll", _searchOption.Value);

                foreach (var file in files)
                {
                    if (AssemblyName.ReferenceMatchesDefinition(AssemblyName.GetAssemblyName(file), assemblyName))
                    {
                        using FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

                        byte[] assemblyBytes = new byte[fs.Length];

                        fs.Read(assemblyBytes, 0, assemblyBytes.Length);

                        assembly = Assembly.Load(assemblyBytes);

                        break;
                    }
                }
            }

            return assembly;
        }
    }
}
