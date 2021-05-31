﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace SpecialPlugin.AspNetCore.Interface
{
    public interface IApplicationWithExternalServiceProvider
    {
        Type StartupModuleType { get; }

        IServiceProvider ServiceProvider { get; }

        IServiceCollection Services { get; }

        IReadOnlyList<IModuleDescriptor> Modules { get; }

        void SetServiceProvider(IServiceProvider serviceProvider);

        void Initialize(IServiceProvider serviceProvider);
    }
}