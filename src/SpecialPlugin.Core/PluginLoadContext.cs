using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SpecialPlugin.Core
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);

            Resolving += PluginLoadContext_Resolving;

            Default.Resolving += (context, assembly) =>
            {
                return Assemblies.FirstOrDefault(p =>
                p.GetName().Name == assembly.Name &&
                p.GetName().Version == assembly.Version);
            };
        }

        private Assembly PluginLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(arg2);

            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
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
