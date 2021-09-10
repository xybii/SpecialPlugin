# SpecialPlugin

.net core 插件化，Api已经尽量Volo.Abp保持一致， 更新了可以使用插件中的Razor具体使用方法请看test的netcoreapp3.1样例

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

## PluginModule

使用Abp需要将 SpecialPlugin.AspNetCore.PluginModule 更换为 Volo.Abp.Modularity.AbpModule

``` csharp
[DependsOn(typeof(JobModule))]
public class Module : PluginModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        var configuration = services.GetConfiguration();

        services.Configure<NewDapperDemoOptions>(configuration.GetSection("NewDapperDemoOptions"));

        services.AddScoped<IJobService, JobService>();

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<BookTag, BookTagDto>();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitPackages", GetType().Namespace, $"wwwroot");

        app.UseFileServer(new FileServerOptions()
        {
            FileProvider = new PhysicalFileProvider(path),   //实际目录地址
            RequestPath = new PathString($"/Resource1"),
            EnableDirectoryBrowsing = true //开启目录浏览
        });

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
        var moudules = Core.PluginExtensions.GetPluginSources<PluginModule>();

        services.AddApplication<HostModule>(o =>
        {
            o.PlugInSources.AddRange(moudules);
        });

        services.AddMvc().ConfigureApplicationPartManager(apm =>
        {
            foreach (var type in moudules) //如果插件里有控制器
            {
                foreach (var part in new DefaultApplicationPartFactory().GetApplicationParts(type.Assembly))
                {
                    apm.ApplicationParts.Add(part);
                }
            }

            foreach (var pluginRazor in Core.PluginExtensions.GetPluginRazors()) //如果插件里有Razor
            {
                apm.ApplicationParts.Add(pluginRazor);
            }
        });

        AddControllers(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.InitializeApplication();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private void AddControllers(IServiceCollection services)
    {
        services.AddOptions();

        services.AddControllers()
          .AddJsonOptions(options =>
          {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
          })
          .AddNewtonsoftJson(options => { options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss"; });

        services.AddControllersWithViews().AddControllersAsServices();
    }
}
```