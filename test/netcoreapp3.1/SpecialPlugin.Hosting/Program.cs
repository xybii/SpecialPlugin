using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace SpecialPlugin.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateGlobalLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog();

        private static void CreateGlobalLogger()
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss:fff} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose() // 所有Sink的最小记录级别
                .Enrich.WithProperty("SourceContext", null) //加入属性SourceContext，也就运行时是调用Logger的具体类
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: outputTemplate)//输出到控制台
                .CreateLogger();
        }
    }
}
