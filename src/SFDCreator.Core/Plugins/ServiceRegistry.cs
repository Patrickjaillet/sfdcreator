namespace SFDCreator.Core.Plugins;

public sealed class ServiceRegistry : IServiceRegistry
{
    private readonly Dictionary<Type, object> _services = new();

    public void Register<TService>(TService instance) where TService : class
    {
        _services[typeof(TService)] = instance;
    }

    public TService Resolve<TService>() where TService : class
    {
        if (TryResolve<TService>(out var instance) && instance is not null)
        {
            return instance;
        }

        throw new InvalidOperationException($"No service registered for type '{typeof(TService).FullName}'.");
    }

    public bool TryResolve<TService>(out TService? instance) where TService : class
    {
        if (_services.TryGetValue(typeof(TService), out var value))
        {
            instance = (TService)value;
            return true;
        }

        instance = null;
        return false;
    }
}
