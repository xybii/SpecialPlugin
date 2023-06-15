using Autofac;

namespace SpecialPlugin.Web.Core
{
    public class AutoFacModule : Module
    {
        private static ILifetimeScope _lifetimeScope;
        private readonly Action<ContainerBuilder> _configure;

        public AutoFacModule(Action<ContainerBuilder> configure)
        {
            _configure = configure;
        }

        protected override void Load(ContainerBuilder builder)
        {
            _configure?.Invoke(builder);

            builder.RegisterBuildCallback(lifetimeScope => _lifetimeScope = lifetimeScope);
        }

        public static ILifetimeScope GetLifetimeScope()
        {
            return _lifetimeScope;
        }
    }
}
