using Microsoft.Extensions.Configuration;
using System;

namespace SpecialPlugin
{
    public class SelectPluginModuleOptions
    {
        public string UnitPackagesName { get; set; } = "UnitPackages";

        public string SearchPackagePattern { get; set; } = "SpecialPlugin.*.dll";

        public IConfigurationRoot ConfigurationRoot { get; set; }
    }
}
