using Ninject;
using Ninject.Activation;

namespace CMM.PolicyFilter.Configuration;

public class NinjectServiceProviderFactory : IServiceProviderFactory<IKernel>
{
    private readonly IKernel _kernel;

    public NinjectServiceProviderFactory()
    {
        this._kernel = new StandardKernel();
    }

    public IKernel CreateBuilder(IServiceCollection services)
    {
        var builtInServiceProvider = services.BuildServiceProvider();

        this._kernel.Bind<IServiceProvider>().ToConstant(builtInServiceProvider).InSingletonScope();

        this._kernel.Bind<IServiceCollection>().ToConstant(services);

        return this._kernel;
    }

    public IServiceProvider CreateServiceProvider(IKernel kernel)
    {
        return new HybridServiceProvider(kernel.Get<IServiceProvider>(), kernel);
    }
}