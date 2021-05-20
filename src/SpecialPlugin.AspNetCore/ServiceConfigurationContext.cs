using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace SpecialPlugin.AspNetCore
{
    public class ServiceConfigurationContext
    {
        public IServiceCollection Services { get; }

        public IDictionary<string, object> Items { get; }

        public object this[string key]
        {
            get => Items.FirstOrDefault(o=> o.Key == key).Value;
            set => Items[key] = value;
        }

        public ServiceConfigurationContext(IServiceCollection services)
        {
            Services = services;
            Items = new Dictionary<string, object>();
        }
    }
}
