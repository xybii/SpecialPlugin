using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Topshelf;

namespace SpecialPlugin.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(name => new MainService(args));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host");

                x.SetDisplayName("SpecialPlugin_Hosting");

                x.SetServiceName("SpecialPlugin_Hosting");
            });
        }
    }
}
