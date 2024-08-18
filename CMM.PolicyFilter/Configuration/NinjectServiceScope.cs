using Ninject;
using Ninject.Syntax;

namespace CMM.PolicyFilter.Configuration;

public class NinjectServiceScope : IServiceScope
{
    private readonly IResolutionRoot _resolutionRoot;

    public NinjectServiceScope(IResolutionRoot resolutionRoot)
    {
        _resolutionRoot = resolutionRoot;
        ServiceProvider = new NinjectServiceProvider((IKernel)resolutionRoot);
    }

    public IServiceProvider ServiceProvider { get; }

    public void Dispose()
    {
        (_resolutionRoot as IDisposable)?.Dispose();
    }
}