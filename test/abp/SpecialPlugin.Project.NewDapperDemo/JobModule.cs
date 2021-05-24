using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Volo.Abp.Modularity;

namespace SpecialPlugin.Project.NewDapperDemo
{
    public class JobModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            var configuration = services.GetConfiguration();

            services.AddQuartz(cfg =>
            {
                cfg.UseMicrosoftDependencyInjectionScopedJobFactory();

                cfg.SchedulerId = "Scheduler-Core-NewDapperDemo";

                string key = "NewDapperDemoJob";

                var jobKey = new JobKey(key);

                cfg.AddJob<IJobService>(jobKey);

                cfg.AddTrigger(t => t
                    .WithIdentity(key)
                    .ForJob(jobKey)
                    .WithCronSchedule("0 0/1 * * * ?").WithDescription(key));
            });

            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
