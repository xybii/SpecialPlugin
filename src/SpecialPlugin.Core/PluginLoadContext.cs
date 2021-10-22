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
        private Assembly BaseAssembly;

        protected static List<PluginLoadContext> PluginLoadContexts;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);

            Resolving += PluginLoadContext_Resolving;

            Default.Resolving += (context, assemblyName) =>
            {
                var assemblyFullName = PluginLoadContexts?.Select(o=> o.BaseAssembly)?.FirstOrDefault(p => p.FullName == assemblyName.FullName);

                if (assemblyFullName != null)
                {
                    return Assemblies.FirstOrDefault(p => p.FullName == assemblyName.FullName);
                }

                return null;
            };
        }

        public void SetBaseAssembly(Assembly assembly)
        {
            BaseAssembly = assembly;
        }

        public void ResgiterContext()
        {
            PluginLoadContexts ??= new List<PluginLoadContext>();

            PluginLoadContexts.Add(this);
        }

        private Assembly Load(Assembly baseAssembly, AssemblyName assemblyName)
        {
            if (baseAssembly == null || assemblyName == null)
            {
                return null;
            }

            Assembly assembly = null;

            var assemblyNames = baseAssembly.GetReferencedAssemblies().Select(o=> o.FullName);

            var contexts = PluginLoadContexts
                .Where(o=> o.BaseAssembly != baseAssembly)
                .Where(o=> o.Assemblies.Any(p=> assemblyNames.Contains(p.FullName)))
                .ToList();

            foreach (var context in contexts)
            {
                if (assembly != null)
                {
                    break;
                }

                assembly = context.Assemblies.FirstOrDefault(o => o.FullName == assemblyName.FullName);

                if (assembly == null)
                {
                    assembly = Load(context.BaseAssembly, assemblyName);
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

            return assembly ?? Load(BaseAssembly, arg2);
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
