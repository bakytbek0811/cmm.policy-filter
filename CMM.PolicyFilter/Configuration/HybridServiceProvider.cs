using Ninject;

namespace CMM.PolicyFilter.Configuration;

public class HybridServiceProvider : IServiceProvider, ISupportRequiredService
{
    private readonly IServiceProvider _builtInProvider;
    private readonly IKernel _kernel;

    public HybridServiceProvider(IServiceProvider builtInProvider, IKernel kernel)
    {
        _builtInProvider = builtInProvider;
        _kernel = kernel;
    }

    public object? GetService(Type serviceType)
    {
        var service = _kernel.TryGet(serviceType);
        return service ?? _builtInProvider.GetService(serviceType);
    }

    public object GetRequiredService(Type serviceType)
    {
        var service = _kernel.TryGet(serviceType);
        return service ?? _builtInProvider.GetRequiredService(serviceType);
    }
}