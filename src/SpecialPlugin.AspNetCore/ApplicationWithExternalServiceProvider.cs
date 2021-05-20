using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin.AspNetCore
{
    public class ApplicationWithExternalServiceProvider : IApplicationWithExternalServiceProvider
    {
        public Type StartupModuleType { get; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IServiceCollection Services { get; }

        public List<IModuleDescriptor> PluginModuleDescriptors { get; }

        public ApplicationWithExternalServiceProvider(
            Type startupModuleType,
            IServiceCollection services,
            Action<ApplicationCreationOptions> optionsAction)
        {
            StartupModuleType = startupModuleType;
            Services = services;

            var options = new ApplicationCreationOptions();

            optionsAction?.Invoke(options);

            PluginModuleDescriptors = LoadModules(services, startupModuleType, options);
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = ServiceProvider;

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = new ApplicationInitializationContext(scope.ServiceProvider);

                foreach (var item in PluginModuleDescriptors)
                {
                    item.Instance.OnApplicationInitialization(context);
                }
            }
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = ServiceProvider;
        }

        public List<IModuleDescriptor> LoadModules(
            IServiceCollection services,
            Type startupModuleType,
            ApplicationCreationOptions options)
        {
            List<Type> types = new List<Type>() { startupModuleType };

            types.AddRange(FindDependedModuleTypes(startupModuleType));

            foreach(var item in options.PlugInSources)
            {
                if (PluginModule.IsPluginModule(item))
                {
                    AddIfNotContains(types, item);

                    foreach (var t in FindDependedModuleTypes(item))
                    {
                        AddIfNotContains(types, t);
                    }
                }
            }

            var moduleDescriptors = new List<ModuleDescriptor>();

            foreach (var type in types)
            {
                var module = (IPluginModule)Activator.CreateInstance(type);

                services.AddSingleton(type, module);

                moduleDescriptors.Add(new ModuleDescriptor(type, module));
            }

            SetDependencies(moduleDescriptors);

            var modules = moduleDescriptors.Cast<IModuleDescriptor>().ToList();

            return SortByDependency(modules, startupModuleType);
        }

        private List<Type> FindDependedModuleTypes(Type moduleType)
        {
            var dependencies = new List<Type>();

            var dependencyDescriptors = moduleType
                .GetCustomAttributes()
                .OfType<IDependedTypesProvider>();

            foreach (var descriptor in dependencyDescriptors)
            {
                foreach (var dependedModuleType in descriptor.GetDependedTypes())
                {
                    AddIfNotContains(dependencies, dependedModuleType);
                }
            }

            return dependencies;
        }

        private void SetDependencies(List<ModuleDescriptor> modules)
        {
            foreach (var module in modules)
            {
                SetDependencies(modules, module);
            }
        }

        private void SetDependencies(List<ModuleDescriptor> modules, ModuleDescriptor module)
        {
            foreach (var dependedModuleType in FindDependedModuleTypes(module.Type))
            {
                var dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType);

                if (dependedModule == null)
                {
                    throw new Exception("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + module.Type.AssemblyQualifiedName);
                }

                module.AddDependency(dependedModule);
            }
        }

        private bool AddIfNotContains<T>(ICollection<T> source, T item)
        {
            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);

            return true;
        }

        private List<IModuleDescriptor> SortByDependency(List<IModuleDescriptor> modules, Type startupModuleType)
        {
            var sortedModules = SortByDependencies(modules, m => m.Dependencies);

            MoveItem(sortedModules, m => m.Type == startupModuleType, modules.Count - 1);

            return sortedModules;
        }

        /// <summary>
        /// Sort a list by a topological sorting, which consider their dependencies.
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="source">A list of objects to sort</param>
        /// <param name="getDependencies">Function to resolve the dependencies</param>
        /// <param name="comparer">Equality comparer for dependencies </param>
        /// <returns>
        /// Returns a new list ordered by dependencies.
        /// If A depends on B, then B will come before than A in the resulting list.
        /// </returns>
        private List<T> SortByDependencies<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, IEqualityComparer<T> comparer = null)
        {
            /* See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
             *      http://en.wikipedia.org/wiki/Topological_sorting
             */

            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>(comparer);

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="item">Item to resolve</param>
        /// <param name="getDependencies">Function to resolve the dependencies</param>
        /// <param name="sorted">List with the sortet items</param>
        /// <param name="visited">Dictionary with the visited items</param>
        private void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }

        public static void MoveItem<T>(List<T> source, Predicate<T> selector, int targetIndex)
        {
            if (!IsBetween(targetIndex, 0, source.Count - 1))
            {
                throw new IndexOutOfRangeException("targetIndex should be between 0 and " + (source.Count - 1));
            }

            var currentIndex = source.FindIndex(0, selector);
            if (currentIndex == targetIndex)
            {
                return;
            }

            var item = source[currentIndex];
            source.RemoveAt(currentIndex);
            source.Insert(targetIndex, item);
        }

        /// <summary>
        /// Checks a value is between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <param name="minInclusiveValue">Minimum (inclusive) value</param>
        /// <param name="maxInclusiveValue">Maximum (inclusive) value</param>
        public static bool IsBetween<T>(T value, T minInclusiveValue, T maxInclusiveValue) where T : IComparable<T>
        {
            return value.CompareTo(minInclusiveValue) >= 0 && value.CompareTo(maxInclusiveValue) <= 0;
        }
    }
}
