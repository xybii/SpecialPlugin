using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace SpecialPlugin.Hosting
{
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    public class HostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            //CreateGlobalLogger();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            app.UseRouting();

            app.UseConfiguredEndpoints();
        }

        private void CreateGlobalLogger()
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss:fff} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", ".log");

#if DEBUG
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose() // 所有Sink的最小记录级别
                .Enrich.WithProperty("SourceContext", null) //加入属性SourceContext，也就运行时是调用Logger的具体类
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose, outputTemplate: outputTemplate)//输出到控制台
                .CreateLogger();
#else
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning() // 所有Sink的最小记录级别
                .Enrich.WithProperty("SourceContext", null) //加入属性SourceContext，也就运行时是调用Logger的具体类
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null,
                shared: true, restrictedToMinimumLevel: LogEventLevel.Warning, outputTemplate: outputTemplate)//输入到文本
                .CreateLogger();
#endif
        }
    }
}
