using SpecialPlugin.AspNetCore.Interface;
using System;

namespace SpecialPlugin.AspNetCore
{
    public class TypePlugInSource : IPlugInSource
    {
        private readonly Type[] _moduleTypes;

        public TypePlugInSource(params Type[] moduleTypes)
        {
            _moduleTypes = moduleTypes ?? new Type[0];
        }

        public Type[] GetModules()
        {
            return _moduleTypes;
        }
    }
}
