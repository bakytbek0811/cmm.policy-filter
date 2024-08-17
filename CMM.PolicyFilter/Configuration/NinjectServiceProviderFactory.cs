using Ninject;
using Ninject.Activation;
using Ninject.Modules;

namespace CMM.PolicyFilter.Configuration;

public class NinjectServiceProviderFactory : IServiceProviderFactory<IKernel>
{
    private readonly IKernel _kernel;

    public NinjectServiceProviderFactory(IConfiguration configuration)
    {
        this._kernel = new StandardKernel(new DataModule(configuration));
    }

    public IKernel CreateBuilder(IServiceCollection services)
    {
        this._kernel.Bind<IServiceCollection>().ToConstant(services);

        return this._kernel;
    }

    public IServiceProvider CreateServiceProvider(IKernel kernel)
    {
        kernel.Bind<IServiceProvider>().ToMethod(ctx => new NinjectServiceProvider(kernel)).InSingletonScope();

        foreach (var service in kernel.Get<IServiceCollection>())
        {
            if (IsSystemService(service.ServiceType))
            {
                continue;
            }

            if (service.ImplementationFactory != null)
            {
                kernel.Bind(service.ServiceType)
                    .ToMethod(ctx => service.ImplementationFactory(ctx.Kernel.Get<IServiceProvider>()))
                    .InScope(GetScope(service.Lifetime));
            }
            else if (service.ImplementationInstance != null)
            {
                kernel.Bind(service.ServiceType).ToConstant(service.ImplementationInstance)
                    .InScope(GetScope(service.Lifetime));
            }
            else if (service.ImplementationType != null)
            {
                kernel.Bind(service.ServiceType).To(service.ImplementationType).InScope(GetScope(service.Lifetime));
            }
            else
            {
                kernel.Bind(service.ServiceType).ToSelf().InScope(GetScope(service.Lifetime));
            }
        }

        return kernel.Get<IServiceProvider>();
    }

    private static Func<IContext, object> GetScope(ServiceLifetime lifetime)
    {
        return lifetime switch
        {
            ServiceLifetime.Singleton => ctx => ctx.Kernel,
            ServiceLifetime.Scoped => ctx => ctx,
            ServiceLifetime.Transient => ctx => null,
            _ => null,
        };
    }

    private static bool IsSystemService(Type serviceType)
    {
        var isSystemService = serviceType == typeof(IHost) ||
                              serviceType == typeof(IHostApplicationLifetime) ||
                              serviceType == typeof(IExternalScopeProvider) ||
                              serviceType == typeof(IConfiguration) ||
                              serviceType.Namespace != null &&
                              serviceType.Namespace.StartsWith("Microsoft.Extensions.");

        return isSystemService;
    }
}