# SpecialPlugin
========================================
.net core 3.1 插件化测试，使用AssemblyLoadContext可在不用的插件中使用不同版本的类库

插件需要放在主程序目录UnitPackages文件夹下，如..\UnitPackages\SpecialPlugin.DapperOneDemo\你的插件dll

## .csproj
------------------------------------------------------------

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
  <Exec Command="xcopy &quot;$(SolutionDir)SpecialPlugin.DapperOneDemo\bin\Debug\netcoreapp3.1&quot; &quot;$(SolutionDir)SpecialPlugin.Hosting\bin\Debug\netcoreapp3.1\UnitPackages\SpecialPlugin.DapperOneDemo&quot; /S /Y /C /E" />
</Target>
```


## PluginModule
------------------------------------------------------------
插件必须在某一个类中继承PluginModule
```
public class StartupModule : PluginModule
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
}
```

## CreateHostBuilder
------------------------------------------------------------
```
public static IHostBuilder CreateHostBuilder(string[] args)
{
    return PluginExtensions.CreateHostBuilder(args, typeof(Startup), "http://*:12000");
}
```
