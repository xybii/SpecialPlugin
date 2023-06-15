using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SpecialPlugin.Core
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private Assembly _baseAssembly;

        internal static List<PluginLoadContext> PluginLoadContexts = new List<PluginLoadContext>();

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);

            Resolving += PluginLoadContext_Resolving;
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

            if (contexts != null)
            {
                foreach (var context in contexts)
                {
                    if (assembly != null)
                    {
                        break;
                    }

                    assembly = context.Assemblies.FirstOrDefault(o => o.FullName == assemblyName.FullName);

                    if (assembly == null)
                    {
                        assembly = Load(context._baseAssembly, assemblyName);
                    }
                }
            }

            return assembly;
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

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
