using Ninject;
using Ninject.Activation;

namespace CMM.PolicyFilter.Configuration
{
    public class NinjectServiceProviderFactory : IServiceProviderFactory<IKernel>
    {
        private readonly IKernel _kernel;

        public NinjectServiceProviderFactory()
        {
            this._kernel = new StandardKernel();
        }

        public IKernel CreateBuilder(IServiceCollection services)
        {
            foreach (var service in services)
            {
                BindService(service);
            }

            _kernel.Bind<IServiceProvider>().ToMethod(_ => new NinjectServiceProvider(_kernel)).InSingletonScope();

            _kernel.Bind<IExternalScopeProvider>().To<LoggerExternalScopeProvider>().InSingletonScope();

            _kernel.Bind<IServiceScopeFactory>().To<NinjectServiceScopeFactory>().InSingletonScope();

            return this._kernel;
        }

        public IServiceProvider CreateServiceProvider(IKernel kernel)
        {
            return kernel.Get<IServiceProvider>();
        }

        private void BindService(ServiceDescriptor service)
        {
            if (service.ImplementationType != null)
            {
                _kernel.Bind(service.ServiceType).To(service.ImplementationType).InScope(GetScope(service.Lifetime));
            }
            else if (service.ImplementationFactory != null)
            {
                _kernel.Bind(service.ServiceType)
                    .ToMethod(ctx => service.ImplementationFactory(ctx.Kernel.Get<IServiceProvider>()))
                    .InScope(GetScope(service.Lifetime));
            }
            else if (service.ImplementationInstance != null)
            {
                _kernel.Bind(service.ServiceType).ToConstant(service.ImplementationInstance)
                    .InScope(GetScope(service.Lifetime));
            }
        }

        private Func<IContext, object> GetScope(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ctx => ctx.Kernel,
                ServiceLifetime.Scoped => ctx => ctx,
                ServiceLifetime.Transient => ctx => null,
                _ => null,
            };
        }
    }
}