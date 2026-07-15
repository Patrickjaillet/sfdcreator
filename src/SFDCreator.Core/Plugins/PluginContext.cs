namespace SFDCreator.Core.Plugins;

public sealed class PluginContext
{
    public required IServiceRegistry Services { get; init; }
}
