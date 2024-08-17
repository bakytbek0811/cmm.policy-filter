using Ninject;

namespace CMM.PolicyFilter.Configuration;

public class NinjectServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
{
    private readonly IKernel _kernel;

    public NinjectServiceProvider(IKernel kernel)
    {
        this._kernel = kernel;
    }

    public object? GetService(Type serviceType)
    {
        return this._kernel.TryGet(serviceType);
    }

    public object GetRequiredService(Type serviceType)
    {
        return this._kernel.Get(serviceType);
    }

    public void Dispose()
    {
        if (_kernel != null)
        {
            _kernel.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}