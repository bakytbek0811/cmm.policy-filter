using Ninject;

namespace CMM.PolicyFilter.Configuration;

public class NinjectServiceScopeFactory : IServiceScopeFactory
{
    private readonly IKernel _kernel;

    public NinjectServiceScopeFactory(IKernel kernel)
    {
        _kernel = kernel;
    }

    public IServiceScope CreateScope()
    {
        return new NinjectServiceScope(_kernel.BeginBlock());
    }
}