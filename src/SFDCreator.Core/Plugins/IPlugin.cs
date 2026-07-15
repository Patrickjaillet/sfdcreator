namespace SFDCreator.Core.Plugins;

public interface IPlugin
{
    string Id { get; }

    string DisplayName { get; }

    Version Version { get; }

    void Initialize(PluginContext context);

    void Shutdown();
}
