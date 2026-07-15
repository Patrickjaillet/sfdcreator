using SFDCreator.Core.Plugins;

namespace SFDCreator.Core.Tests.Plugins;

public sealed class PluginManagerTests
{
    [Fact]
    public void RegisterPlugin_InitializesAndTracksThePlugin()
    {
        var manager = new PluginManager(new ServiceRegistry());
        var plugin = new StubPlugin();

        manager.RegisterPlugin(plugin);

        Assert.Single(manager.LoadedPlugins);
        Assert.True(plugin.Initialized);
    }

    [Fact]
    public void ShutdownAll_ShutsDownEveryLoadedPlugin()
    {
        var manager = new PluginManager(new ServiceRegistry());
        var plugin = new StubPlugin();
        manager.RegisterPlugin(plugin);

        manager.ShutdownAll();

        Assert.True(plugin.ShutDown);
        Assert.Empty(manager.LoadedPlugins);
    }

    private sealed class StubPlugin : IPlugin
    {
        public bool Initialized { get; private set; }

        public bool ShutDown { get; private set; }

        public string Id => "stub";

        public string DisplayName => "Stub Plugin";

        public Version Version => new(1, 0, 0);

        public void Initialize(PluginContext context) => Initialized = true;

        public void Shutdown() => ShutDown = true;
    }
}
