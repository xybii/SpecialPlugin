using System;
using System.Diagnostics;
using System.IO;
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

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);

            string path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly?.Location), $"{assemblyName.Name}.dll");

            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll");

            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(args.RequestingAssembly?.Location));

            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly?.Location), Path.GetFileName(args.RequestingAssembly?.Location));

            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            throw new DllNotFoundException();
        }

        private Assembly PluginLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(arg2);

            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return Default.LoadFromAssemblyName(arg2);
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
