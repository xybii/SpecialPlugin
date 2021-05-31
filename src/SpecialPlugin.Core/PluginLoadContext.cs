using System;
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

            if (assemblyName.Version == null)
            {
                if (PluginOptions.ShowTips)
                {
                    Console.WriteLine($"Loading failed:{args.Name}");
                }

                return null;
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll");

            if (File.Exists(path))
            {
                if (PluginOptions.ShowTips)
                {
                    Console.WriteLine($"LoadFrom:{path}");
                }

                return Assembly.LoadFrom(path);
            }

            if (!string.IsNullOrEmpty(args.RequestingAssembly?.Location))
            {
                path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly?.Location), $"{assemblyName.Name}.dll");

                if (File.Exists(path))
                {
                    if (PluginOptions.ShowTips)
                    {
                        Console.WriteLine($"LoadFrom:{path}");
                    }

                    return Assembly.LoadFrom(path);
                }

                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(args.RequestingAssembly?.Location));

                if (File.Exists(path))
                {
                    if (PluginOptions.ShowTips)
                    {
                        Console.WriteLine($"LoadFrom:{path}");
                    }

                    return Assembly.LoadFrom(path);
                }

                path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly?.Location), Path.GetFileName(args.RequestingAssembly?.Location));

                if (File.Exists(path))
                {
                    if (PluginOptions.ShowTips)
                    {
                        Console.WriteLine($"LoadFrom:{path}");
                    }

                    return Assembly.LoadFrom(path);
                }
            }

            return null;
        }

        private Assembly PluginLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(arg2);

            if (assemblyPath != null)
            {
                if (PluginOptions.ShowTips)
                {
                    Console.WriteLine($"LoadFromAssemblyPath:{assemblyPath}");
                }

                return LoadFromAssemblyPath(assemblyPath);
            }

            var assembly = Default.LoadFromAssemblyName(arg2);

            if(assembly != null)
            {
                if (PluginOptions.ShowTips)
                {
                    Console.WriteLine($"LoadFromAssemblyName:{assembly.Location}");
                }
            }

            return assembly ?? null;
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
