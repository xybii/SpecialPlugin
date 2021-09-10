using Quartz;
using SpecialPlugin.AspNetCore;

namespace SpecialPlugin.Project.OldDapperDemo
{
    public class JobModule : PluginModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            var configuration = services.GetConfiguration();

            services.AddQuartz(cfg =>
            {
                cfg.UseMicrosoftDependencyInjectionJobFactory();

                cfg.SchedulerId = "Scheduler-Core-OldDapperDemo";

                string key = "OldDapperDemoJob";

                var jobKey = new JobKey(key);

                cfg.AddJob<IJobService>(jobKey);

                cfg.AddTrigger(t => t
                    .WithIdentity(key)
                    .ForJob(jobKey)
                    .WithCronSchedule("0 0/1 * * * ?").WithDescription(key));
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
