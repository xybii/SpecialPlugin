using System;
using System.Collections.Generic;

namespace SpecialPlugin.AspNetCore
{
    public class ApplicationCreationOptions
    {
        public List<Type> PlugInSources { get; }

        public ApplicationCreationOptions()
        {
            PlugInSources = new List<Type>();
        }
    }
}
