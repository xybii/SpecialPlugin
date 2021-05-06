using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecialPlugin
{
    public class PluginStartupFilter : IStartupFilter
    {
        public List<Action<IApplicationBuilder>> ActionList = new List<Action<IApplicationBuilder>>();

        public void AddConfigureAction(Action<IApplicationBuilder> action)
        {
            ActionList.Add(action);
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);

                foreach (var item in ActionList.Where(o => o != null))
                {
                    item.Invoke(builder);
                }
            };
        }
    }
}