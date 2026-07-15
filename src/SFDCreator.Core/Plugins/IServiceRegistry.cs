namespace SFDCreator.Core.Plugins;

public interface IServiceRegistry
{
    void Register<TService>(TService instance) where TService : class;

    TService Resolve<TService>() where TService : class;

    bool TryResolve<TService>(out TService? instance) where TService : class;
}
