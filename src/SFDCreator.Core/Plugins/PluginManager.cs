using System.Reflection;

namespace SFDCreator.Core.Plugins;

public sealed class PluginManager
{
    private readonly List<IPlugin> _loadedPlugins = new();
    private readonly IServiceRegistry _services;

    public PluginManager(IServiceRegistry services)
    {
        _services = services;
    }

    public IReadOnlyList<IPlugin> LoadedPlugins => _loadedPlugins;

    public void LoadFromDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        foreach (var assemblyPath in Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.TopDirectoryOnly))
        {
            LoadFromAssembly(Assembly.LoadFrom(assemblyPath));
        }
    }

    public void LoadFromAssembly(Assembly assembly)
    {
        var pluginTypes = assembly.GetTypes()
            .Where(type => typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        foreach (var pluginType in pluginTypes)
        {
            if (Activator.CreateInstance(pluginType) is not IPlugin plugin)
            {
                continue;
            }

            RegisterPlugin(plugin);
        }
    }

    public void RegisterPlugin(IPlugin plugin)
    {
        plugin.Initialize(new PluginContext { Services = _services });
        _loadedPlugins.Add(plugin);
    }

    public void ShutdownAll()
    {
        foreach (var plugin in _loadedPlugins)
        {
            plugin.Shutdown();
        }

        _loadedPlugins.Clear();
    }
}
