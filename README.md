# SpecialPlugin

.net core 3.1 插件化测试，使用AssemblyLoadContext可在不用的插件中使用不同版本的类库

## .csproj

插件必需配置

``` csharp
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

``` csharp
xcopy "$(SolutionDir)src\SpecialPlugin.DapperOneDemo\bin\Debug\netcoreapp3.1" "$(SolutionDir)src\SpecialPlugin.Hosting\bin\Debug\netcoreapp3.1\UnitPackages\SpecialPlugin.DapperOneDemo" /S /Y /C /E
```


## PluginModule

插件必须在某一个类中继承PluginModule

``` csharp
public class StartupModule : PluginModule, IRegisterAutoMapper, IRegisterQuartz
{
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
}
```

## CreateHostBuilder

``` csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
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
}
```
