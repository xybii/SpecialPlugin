﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SpecialPlugin.AspNetCore;
using SpecialPlugin.Project.OldDapperDemo;

namespace SpecialPlugin.Project.ReplaceController
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Scoped<ITest, NTest>());
        }
    }
}
