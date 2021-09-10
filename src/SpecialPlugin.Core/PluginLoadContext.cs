using Microsoft.AspNetCore.Mvc.ApplicationParts;
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

        public static bool ShowTips { get; set; }

        public static List<PluginLoadContext> PluginLoadContexts { get; protected set; } = new List<PluginLoadContext>();

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);

            Resolving += PluginLoadContext_Resolving;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Default.Resolving += (context, assembly) =>
            {
                return Assemblies.FirstOrDefault(p =>
                p.GetName().Name == assembly.Name &&
                p.GetName().Version == assembly.Version);
            };
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assembly = CurrentDomain_AssemblyResolve(args, out string writeLineStr);

            if (ShowTips && !string.IsNullOrEmpty(writeLineStr))
            {
                Console.WriteLine(writeLineStr);
            }

            return assembly;
        }

        private static Assembly CurrentDomain_AssemblyResolve(ResolveEventArgs args, out string writeLineStr)
        {
            var assemblyName = new AssemblyName(args.Name);

            if (assemblyName.Version == null)
            {
                writeLineStr = $"Skip loading:{args.Name}";

                return null;
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll");

            if (File.Exists(path))
            {
                writeLineStr = $"LoadFrom:{path}";

                return Assembly.LoadFrom(path);
            }

            if (!string.IsNullOrEmpty(args.RequestingAssembly?.Location))
            {
                path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly?.Location), $"{assemblyName.Name}.dll");

                if (File.Exists(path))
                {
                    writeLineStr = $"LoadFrom:{path}";

                    return Assembly.LoadFrom(path);
                }

                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(args.RequestingAssembly?.Location));

                if (File.Exists(path))
                {
                    writeLineStr = $"LoadFrom:{path}";

                    return Assembly.LoadFrom(path);
                }

                path = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly?.Location), Path.GetFileName(args.RequestingAssembly?.Location));

                if (File.Exists(path))
                {
                    writeLineStr = $"LoadFrom:{path}";

                    return Assembly.LoadFrom(path);
                }
            }

            writeLineStr = $"Loading failed:{args.Name}";

            return null;
        }

        private Assembly PluginLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            var assembly = PluginLoadContext_Resolving(arg2, out var writeLineStr);

            if (ShowTips && !string.IsNullOrEmpty(writeLineStr))
            {
                Console.WriteLine(writeLineStr);
            }

            return assembly;
        }

        private Assembly PluginLoadContext_Resolving(AssemblyName arg, out string writeLineStr)
        {
            writeLineStr = null;

            string assemblyPath = _resolver.ResolveAssemblyToPath(arg);

            if (assemblyPath != null)
            {
                writeLineStr = $"LoadFromAssemblyPath:{assemblyPath}";

                return LoadFromAssemblyPath(assemblyPath);
            }

            var assembly = Assemblies.FirstOrDefault(o =>
            o.GetName().Name == arg.Name &&
            o.GetName().Version == arg.Version) == null ?
            null : Default.LoadFromAssemblyName(arg);

            if (assembly != null)
            {
                writeLineStr = $"LoadFromAssemblyName:{assembly.Location}";
            }

            return assembly;
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
