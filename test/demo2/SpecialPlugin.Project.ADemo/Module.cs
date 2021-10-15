using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.AspNetCore;

namespace SpecialPlugin.Project.ADemo
{
    public class Module : PluginModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITest, Test>();
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication(); //必须在上

            app.UseAuthorization();
        }
    }
}
