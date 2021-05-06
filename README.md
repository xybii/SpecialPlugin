# SpecialPlugin

.net core 3.1 插件化测试，使用AssemblyLoadContext可在不用的插件中使用不同版本的类库

## .csproj

插件必需配置

```
<PropertyGroup>
  <TargetFramework>netcoreapp3.1</TargetFramework>
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
...
<ItemGroup>
  <ProjectReference Include="..\SpecialPlugin\SpecialPlugin.csproj">
    <Private>false</Private>
    <ExcludeAssets>runtime</ExcludeAssets>
  </ProjectReference>
</ItemGroup>
```

xcopy样例
```
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
<<<<<<< HEAD
  <Exec Command="xcopy &quot;$(SolutionDir)src\SpecialPlugin.DapperOneDemo\bin\Debug\netcoreapp3.1&quot; &quot;$(SolutionDir)src\SpecialPlugin.Hosting\bin\Debug\netcoreapp3.1\UnitPackages\SpecialPlugin.DapperOneDemo&quot; /S /Y /C /E" />
=======
  <Exec Command="xcopy &quot;$(SolutionDir)SpecialPlugin.DapperOneDemo\bin\Debug\netcoreapp3.1&quot; &quot;$(SolutionDir)SpecialPlugin.Hosting\bin\Debug\netcoreapp3.1\UnitPackages\SpecialPlugin.DapperOneDemo&quot; /S /Y /C /E" />
>>>>>>> f30f729a0d6201dbc670e822dbc3d04fe7fe33f7
</Target>
```


## PluginModule

插件必须在某一个类中继承PluginModule
```
public class StartupModule : PluginModule, IRegisterAutoMapper, IRegisterQuartz
{
<<<<<<< HEAD
    private readonly string _name;

    public StartupModule(IConfiguration configuration) : base(configuration)
    {
        _name = "DapperOneDemoJob";
    }

    public override void RegisterAssemblyTypes(ContainerBuilder containerBuilder)
    {
        Console.WriteLine($"{_name},RegisterAssemblyTypes");

        Assembly assembly = Assembly.GetExecutingAssembly();

        AutoFacModule autoFacModule = new AutoFacModule(o =>
        {
          o.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().InstancePerLifetimeScope();
        });

        containerBuilder.RegisterModule(autoFacModule);
    }

    public void RegisterAutoMapperConfigure(IMapperConfigurationExpression mapExpression)
    {
        Console.WriteLine($"{_name},RegisterAutoMapper");

        mapExpression.CreateMap<BookTag, BookTagDto>();
    }

    public override void RegisterConfigure(IApplicationBuilder app)
    {
        Console.WriteLine($"{_name},RegisterConfigure");
      
        using (var scope = app.ApplicationServices.CreateScope())
        {
          scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
        }
    }

    public override void RegisterConfigureServices(IServiceCollection services)
    {
        Console.WriteLine($"{_name},RegisterConfigureServices");

        services.Configure<DapperOneDemoOptions>(Configuration.GetSection("DapperOneDemoOptions"));
    }

    public void RegisterQuartzConfigure(IServiceCollectionQuartzConfigurator configurator)
    {
        Console.WriteLine($"{_name},RegisterQuartzJob");

        string key = "DapperOneDemoJob";

        var jobKey = new JobKey(key);

        configurator.AddJob<IJobService>(jobKey);

        configurator.AddTrigger(t => t
            .WithIdentity(key)
            .ForJob(jobKey)
            .WithCronSchedule("0 0/1 * * * ?").WithDescription(key)
      );
  }
=======
   private readonly string _name;

   public StartupModule(IConfiguration configuration) : base(configuration)
   {
     _name = "DapperOneDemoJob";
   }

   public override void RegisterAssemblyTypes(ContainerBuilder containerBuilder)
   {
     Console.WriteLine($"{_name},RegisterAssemblyTypes");

     Assembly assembly = Assembly.GetExecutingAssembly();

     AutoFacModule autoFacModule = new AutoFacModule(o =>
     {
         o.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().InstancePerLifetimeScope();
     });

     containerBuilder.RegisterModule(autoFacModule);
   }

   public override void RegisterAutoMapper(IMapperConfigurationExpression mapExpression)
   {
     Console.WriteLine($"{_name},RegisterAutoMapper");

     mapExpression.CreateMap<BookTag, BookTagDto>();
   }

   public override void RegisterConfigure(IApplicationBuilder app)
   {
     Console.WriteLine($"{_name},RegisterConfigure");

     using (var scope = app.ApplicationServices.CreateScope())
     {
         scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
     }
   }

   public override void RegisterConfigureServices(IServiceCollection services)
   {
     Console.WriteLine($"{_name},RegisterConfigureServices");

     services.Configure<DapperOneDemoOptions>(Configuration.GetSection("DapperOneDemoOptions"));
   }

   public override void RegisterQuartzJob(IServiceCollectionQuartzConfigurator configurator)
   {
     Console.WriteLine($"{_name},RegisterQuartzJob");

     string key = "DapperOneDemoJob";

     var jobKey = new JobKey(key);

     configurator.AddJob<IJobService>(jobKey);

     configurator.AddTrigger(t => t
       .WithIdentity(key)
       .ForJob(jobKey)
       .WithCronSchedule("0 0/1 * * * ?").WithDescription(key)
     );
   }
>>>>>>> f30f729a0d6201dbc670e822dbc3d04fe7fe33f7
}
```

## CreateHostBuilder

```
public static IHostBuilder CreateHostBuilder(string[] args)
{
<<<<<<< HEAD
    var modules = PluginExtensions.SelectPluginModule(options =>
    {
        options.ConfigurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).Build();
    });

    return PluginExtensions.CreateHostBuilder(args, modules,
        services =>
        {
            services.AddPluginAutoMapper(modules, true);

            services.AddPluginQuartz(modules);
        },
        webBuilder =>
        {
            webBuilder.UseStartup<Startup>();

            webBuilder.UseUrls("http://*:15555");
        });
=======
    return PluginExtensions.CreateHostBuilder(args, typeof(Startup), "http://*:12000");
>>>>>>> f30f729a0d6201dbc670e822dbc3d04fe7fe33f7
}
```
