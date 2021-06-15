# SpecialPlugin

.net core 插件化，Api已经尽量Volo.Abp保持一致

SpecialPlugin.AspNetCore 此包只做少量功能的移植，实现请参考 https://github.com/abpframework/abp

## Packages

--------
| Package | NuGet |
| ------- | ------------ |
| [SpecialPlugin.Core](https://www.nuget.org/packages/SpecialPlugin.Core/) | [![SpecialPlugin.Core](https://img.shields.io/nuget/v/SpecialPlugin.Core.svg)](https://www.nuget.org/packages/SpecialPlugin.Core/) |
| [SpecialPlugin.AspNetCore](https://www.nuget.org/packages/SpecialPlugin.AspNetCore/) | [![SpecialPlugin.AspNetCore](https://img.shields.io/nuget/v/SpecialPlugin.AspNetCore.svg)](https://www.nuget.org/packages/SpecialPlugin.AspNetCore/) |

## .csproj

### 插件需要配置

``` csharp
<PropertyGroup>
  <TargetFramework>net5.0</TargetFramework>
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies> <!--生成插件所需要的依赖-->
</PropertyGroup>
```

### 使用SpecialPlugin.AspNetCore进行插件开发需要配置

```  csharp
<ItemGroup>
  <PackageReference Include="SpecialPlugin.AspNetCore" Version="0.0.2">
    <Private>false</Private>
    <ExcludeAssets>runtime</ExcludeAssets>
  </PackageReference>
</ItemGroup>
```

### 使用Volo.Abp进行插件开发需要配置

```  csharp
<ItemGroup>
  <PackageReference Include="Volo.Abp.AspNetCore.Mvc" Version="4.3.1">
    <Private>false</Private>
    <ExcludeAssets>runtime</ExcludeAssets>
  </PackageReference>
</ItemGroup>
```

### xcopy样例

``` csharp
xcopy "$(SolutionDir)test\net5.0\SpecialPlugin.Project.NewDapperDemo\bin\Debug\net5.0" "$(SolutionDir)test\net5.0\SpecialPlugin.Hosting\bin\Debug\net5.0\UnitPackages\SpecialPlugin.Project.NewDapperDemo" /S /E /Y /C /I /V /D
```

## PluginModule

使用Abp需要将 SpecialPlugin.AspNetCore.PluginModule 更换为 Volo.Abp.Modularity.AbpModule

``` csharp
[DependsOn(typeof(JobModule))]
public class Module : SpecialPlugin.AspNetCore.PluginModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        var configuration = services.GetConfiguration();

        services.Configure<NewDapperDemoOptions>(configuration.GetSection("NewDapperDemoOptions"));

        services.AddScoped<IJobService, JobService>();

        services.AddMvc().ConfigureApplicationPartManager(apm =>
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(assembly))
            {
                apm.ApplicationParts.Add(part);
            }

            foreach (var part in new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(assembly))
            {
                apm.ApplicationParts.Add(part);
            }
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<BookTag, BookTagDto>();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        using (var scope = app.ApplicationServices.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IJobService>().Execute(null).GetAwaiter().GetResult();
        }
    }
}
```

## Startup

使用Abp需要将 SpecialPlugin.AspNetCore.PluginModule 更换为 Volo.Abp.Modularity.AbpModule

``` csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var moudules = SpecialPlugin.Core.PluginExtensions.GetPlugInSources<SpecialPlugin.AspNetCore.PluginModule>();

        services.AddApplication<HostModule>(o =>
        {
            o.PlugInSources.AddRange(moudules);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.InitializeApplication();
    }
}
```