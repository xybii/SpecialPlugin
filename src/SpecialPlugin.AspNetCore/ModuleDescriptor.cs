using SpecialPlugin.AspNetCore.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SpecialPlugin.AspNetCore
{
    public class ModuleDescriptor : IModuleDescriptor
    {
        public Type Type { get; }

        public IPluginModule Instance { get; }

        public IReadOnlyList<IModuleDescriptor> Dependencies => _dependencies.ToImmutableList();

        private readonly List<IModuleDescriptor> _dependencies;

        public ModuleDescriptor(Type type, IPluginModule instance)
        {
            Type = type;
            Instance = instance;

            _dependencies = new List<IModuleDescriptor>();
        }

        public void AddDependency(IModuleDescriptor descriptor)
        {
            if (!_dependencies.Contains(descriptor))
            {
                _dependencies.Add(descriptor);
            }
        }
    }
}
