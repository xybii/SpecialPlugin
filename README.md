# SpecialPlugin

.net core 插件化实现

SpecialPlugin.AspNetCore 此包只做少量功能的移植，实现请参考 <https://github.com/abpframework/abp>

## Packages

--------
| Package | NuGet |
| ------- | ------------ |
| [SpecialPlugin.Core](https://www.nuget.org/packages/SpecialPlugin.Core/) | [![SpecialPlugin.Core](https://img.shields.io/nuget/v/SpecialPlugin.Core.svg)](https://www.nuget.org/packages/SpecialPlugin.Core/) |
| [SpecialPlugin.AspNetCore](https://www.nuget.org/packages/SpecialPlugin.AspNetCore/) | [![SpecialPlugin.AspNetCore](https://img.shields.io/nuget/v/SpecialPlugin.AspNetCore.svg)](https://www.nuget.org/packages/SpecialPlugin.AspNetCore/) |

## 样例

[在插件中替换控制器方法](https://github.com/xybii/SpecialPlugin/blob/main/test/netcoreapp3.1/SpecialPlugin.Project.NewDapperDemo/Controllers/NewController.cs)

[在插件中替换已注册的类](https://github.com/xybii/SpecialPlugin/blob/main/test/netcoreapp3.1/SpecialPlugin.Project.ReplaceReplaceController/Module.cs)

[在插件中新增控制器页面](https://github.com/xybii/SpecialPlugin/blob/main/test/netcoreapp3.1/SpecialPlugin.Project.NewDapperDemo/Module.cs)

## .csproj

### 插件需要配置

``` csharp
<Project Sdk="Microsoft.NET.Sdk"> <!--如果需要razor视图则修改为Microsoft.NET.Sdk.Web-->
<PropertyGroup>
 <TargetFramework>netcoreapp3.1</TargetFramework>
 <OutputType>Library</OutputType> <!--如果需要razor视图-->
 <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies> <!--必须配置-->
</PropertyGroup>
```

### 主程序需要配置

```  csharp
<ItemGroup>
  <PackageReference Include="SpecialPlugin.Core" Version="1.1.0" />
</ItemGroup>
```

### 使用SpecialPlugin.AspNetCore进行插件开发需要配置

```  csharp
<ItemGroup>
  <PackageReference Include="SpecialPlugin.AspNetCore" Version="1.0.0">
    <Private>false</Private>
    <ExcludeAssets>runtime</ExcludeAssets>
  </PackageReference>
</ItemGroup>
```

### 使用Volo.Abp进行插件开发需要配置

```  csharp
<ItemGroup>
  <PackageReference Include="Volo.Abp.AspNetCore.Mvc" Version="4.3.2">
    <Private>false</Private>
    <ExcludeAssets>runtime</ExcludeAssets>
  </PackageReference>
</ItemGroup>
```

### xcopy样例

``` csharp
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <Exec Command="xcopy &quot;$(SolutionDir)test\netcoreapp3.1\SpecialPlugin.Project.NewDapperDemo\bin\Debug\netcoreapp3.1&quot; &quot;$(SolutionDir)test\netcoreapp3.1\SpecialPlugin.Hosting\bin\Debug\netcoreapp3.1\UnitPackages\SpecialPlugin.Project.NewDapperDemo&quot; /S /E /Y /C /I /V /D" />
</Target>
```

插件文件夹中refs文件夹是不需要复制的，如果希望在xcopy复制时屏蔽refs文件夹，可在目录下创建exclusion.txt

```
\refs\
```

将xcopy脚本添加 /EXCLUDE:$(SolutionDir)exclusion.txt

``` csharp
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <Exec Command="xcopy &quot;$(SolutionDir)test\netcoreapp3.1\SpecialPlugin.Project.NewDapperDemo\bin\Debug\netcoreapp3.1&quot; &quot;$(SolutionDir)test\netcoreapp3.1\SpecialPlugin.Hosting\bin\Debug\netcoreapp3.1\UnitPackages\SpecialPlugin.Project.NewDapperDemo&quot; /S /E /Y /C /I /V /D /EXCLUDE:$(SolutionDir)exclusion.txt" />
</Target>
```

## Module

### 原生

``` csharp
public class Module : StartupModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
    }
    
    public override void Configure(IApplicationBuilder app)
    {
    }
}
```

### PluginModule

``` csharp
public class Module : SpecialPlugin.AspNetCore.PluginModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        var configuration = services.GetConfiguration();
    }
    
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
    }
}
```

### AbpModule

``` csharp
public class Module : Volo.Abp.Modularity.AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        var configuration = services.GetConfiguration();
    }
    
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
    }
}
```

## Startup

### 原生

``` csharp
public class Startup
{
    public List<StartupModule> PluginModules = new List<StartupModule>();

    public void ConfigureServices(IServiceCollection services)
    {
        var modules = Core.PluginExtensions.GetPluginSources<StartupModule>();

        foreach(var module in modules)
        {
            var pluginModele = Activator.CreateInstance(module) as StartupModule;

            pluginModele.ConfigureServices(services);

            PluginModules.Add(pluginModele);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
           app.UseDeveloperExceptionPage();
        }

        foreach (var pluginModele in PluginModules)
        {
           pluginModele.Configure(app);
        }
    }
}
```

### Abp

``` csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var modules = Core.PluginExtensions.GetPluginSources<AbpModule>().ToArray();

        services.AddApplication<HostModule>(o =>
        {
            o.PlugInSources.AddTypes(modules);
        });

        services.AddMvc().ConfigureApplicationPartManager(apm =>
        {
            foreach (var type in modules)
            {
                foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(type.Assembly))
                {
                        apm.ApplicationParts.Add(part);
                }
            }

            foreach (var pluginRazor in Core.PluginExtensions.GetPluginRazors())
            {
                apm.ApplicationParts.Add(pluginRazor);
            }
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.InitializeApplication();
    }
}
```
