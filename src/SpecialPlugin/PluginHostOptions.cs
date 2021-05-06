using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace SpecialPlugin
{
    public class PluginHostOptions
    {
        public string[] Args { get; set; }

        public Type StarupType { get; set; }

        public IList<PluginModule> PluginModules { get; set; }

        public Action<IServiceCollection> ServiceConfigAction { get; set; }

        public IList<Action<IWebHostBuilder>> WebHostConfigActions { get; set; }
    }
}
