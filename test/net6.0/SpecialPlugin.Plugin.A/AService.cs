using Autofac.Features.OwnedInstances;
using Autofac;
using SpecialPlugin.Plugin.B;
using SpecialPlugin.Web.Core;

namespace SpecialPlugin.Plugin.A
{
    public class AService
    {
        public AService()
        {
            using (var scope = AutoFacModule.GetLifetimeScope().BeginLifetimeScope())
            {
                var bService = scope.Resolve<Func<Owned<BService>>>().Invoke().Value;

                bService.Test();
            }

            Test();
        }

        public void Test()
        {
            Console.WriteLine("A");
        }
    }
}
