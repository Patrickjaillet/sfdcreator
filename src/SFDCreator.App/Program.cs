using SFDCreator.Core.Plugins;
using SFDCreator.Rendering.Windowing;
using SFDCreator.UI.Rendering;
using Silk.NET.OpenGL;

var services = new ServiceRegistry();
var pluginManager = new PluginManager(services);
pluginManager.LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "Plugins"));

var skiaHost = new SkiaSurfaceHost();

using var window = new RenderWindow(new RenderWindowOptions
{
    Title = "SFD Creator",
    Width = 1280,
    Height = 720,
});

window.GLReady += gl => gl.ClearColor(0.05f, 0.05f, 0.08f, 1.0f);
window.RenderFrame += (_, gl) => gl.Clear((uint)ClearBufferMask.ColorBufferBit);

window.Run();

pluginManager.ShutdownAll();
skiaHost.Dispose();
