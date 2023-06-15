using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpecialPlugin.Web.Core;

namespace SpecialPlugin.Project.ADemo
{
    public class Module : StartupModule
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
