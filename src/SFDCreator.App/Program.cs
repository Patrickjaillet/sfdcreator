using System.Numerics;
using SFDCreator.App;
using SFDCreator.Core.Plugins;
using SFDCreator.Core.Settings;
using SFDCreator.IO.Settings;
using SFDCreator.Rendering.Backend;
using SFDCreator.Rendering.Cameras;
using SFDCreator.Rendering.Diagnostics;
using SFDCreator.Rendering.Graph;
using SFDCreator.Rendering.OpenGL;
using SFDCreator.Rendering.PostProcessing;
using SFDCreator.Rendering.Resources;
using SFDCreator.Win32;
using SFDCreator.Win32.Dialogs;
using SFDCreator.Win32.Docking;
using SFDCreator.Win32.Menu;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using VertexArray = SFDCreator.Rendering.Resources.VertexArray;

const int CommandFileOpen = 1001;
const int CommandFileSave = 1002;
const int CommandFileExit = 1003;
const int CommandHelpAbout = 2001;
const float CameraPathDuration = 12f;

var services = new ServiceRegistry();
var pluginManager = new PluginManager(services);
pluginManager.LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "Plugins"));

ISettingsStore settingsStore = new RegistrySettingsStore();
var settings = settingsStore.Load();

var menu = new NativeMenuBuilder()
    .AddTopLevel("&File")
    .AddItem("&Open...", CommandFileOpen)
    .AddItem("&Save As...", CommandFileSave)
    .AddItem("E&xit", CommandFileExit)
    .AddTopLevel("&Help")
    .AddItem("&About", CommandHelpAbout);

using var window = new Win32Window(new Win32WindowOptions
{
    Title = "SFD Creator",
    Width = settings.WindowWidth,
    Height = settings.WindowHeight,
    X = settings.WindowX >= 0 ? settings.WindowX : null,
    Y = settings.WindowY >= 0 ? settings.WindowY : null,
    Maximized = settings.WindowMaximized,
    MenuBuilder = menu,
});

services.Register<IInputContext>(window.Input);

var dockPanels = new DockPanelHost(window);

window.FilesDropped += files =>
{
    foreach (var file in files)
    {
        Console.WriteLine($"Dropped: {file}");
    }
};

window.MenuCommand += commandId =>
{
    switch (commandId)
    {
        case CommandFileOpen:
            Win32FileDialog.OpenFile(window.Handle, "Open Project", "SFD Project (*.sfdproj)|*.sfdproj|All Files (*.*)|*.*");
            break;

        case CommandFileSave:
            Win32FileDialog.SaveFile(window.Handle, "Save Project", "SFD Project (*.sfdproj)|*.sfdproj|All Files (*.*)|*.*");
            break;

        case CommandFileExit:
            window.Close();
            break;

        case CommandHelpAbout:
            NativeMessageBox.Show(window.Handle, "SFD Creator\nCopyright (c) 2026 SANDEFJORD DEVELOPMENT", "About SFD Creator");
            break;
    }
};

window.Closing += () =>
{
    var bounds = window.GetWindowBounds();
    settingsStore.Save(settings with
    {
        WindowX = bounds.X,
        WindowY = bounds.Y,
        WindowWidth = bounds.Width,
        WindowHeight = bounds.Height,
    });
};

var centerPanelHandle = dockPanels.GetPanelHandle(DockRegion.Center);
var device = (OpenGlDevice)GraphicsDeviceFactory.Create(GraphicsBackendKind.OpenGL, centerPanelHandle, new GraphicsDeviceOptions());
device.MakeCurrent();

var gl = device.Api;
gl.Enable(EnableCap.DepthTest);

var shaderDirectory = Path.Combine(AppContext.BaseDirectory, "PostProcessing", "Content", "Shaders");

var (initialWidth, initialHeight) = dockPanels.GetPanelSize(DockRegion.Center);
initialWidth = Math.Max(initialWidth, 1);
initialHeight = Math.Max(initialHeight, 1);

var sceneTarget = new RenderTarget(gl, (uint)initialWidth, (uint)initialHeight);
var postChain = new PostProcessChain(gl, shaderDirectory, (uint)initialWidth, (uint)initialHeight);

var sceneShader = ShaderLoader.Load(gl, shaderDirectory, "scene.vert", "scene.frag");
var (cubeVertexBuffer, cubeIndexBuffer, cubeIndexCount) = CubeGeometry.Create(gl);
var cubeVao = new VertexArray(gl, cubeVertexBuffer, cubeIndexBuffer, (uint)(sizeof(float) * 6), new[]
{
    new VertexAttribute(0, 3, 0),
    new VertexAttribute(1, 3, sizeof(float) * 3u),
});

var camera = new PerspectiveCamera
{
    AspectRatio = initialWidth / (float)initialHeight,
};

var cameraPath = new CameraPathAnimator(new[]
{
    new CameraPathKeyframe(0f, new Vector3(0f, 1.5f, 4f), Vector3.Zero),
    new CameraPathKeyframe(3f, new Vector3(3.5f, 1f, 0f), Vector3.Zero),
    new CameraPathKeyframe(6f, new Vector3(0f, 1.5f, -4f), Vector3.Zero),
    new CameraPathKeyframe(9f, new Vector3(-3.5f, 1f, 0f), Vector3.Zero),
    new CameraPathKeyframe(12f, new Vector3(0f, 1.5f, 4f), Vector3.Zero),
});

var scenePass = new ScenePass(sceneShader, cubeVao, cubeIndexCount) { Writes = new[] { sceneTarget } };

var graph = new RenderGraph();
graph.AddPass(scenePass);

var clock = new FrameClock();
var stats = new FrameStats();

dockPanels.PanelResized += (region, width, height) =>
{
    if (region != DockRegion.Center)
    {
        return;
    }

    width = Math.Max(width, 1);
    height = Math.Max(height, 1);

    device.MakeCurrent();
    sceneTarget.Resize((uint)width, (uint)height);
    postChain.Resize((uint)width, (uint)height);
    camera.AspectRatio = width / (float)height;
};

window.RunWithIdle(() =>
{
    device.MakeCurrent();

    var delta = clock.Tick();
    stats.Record(delta);

    cameraPath.Apply(camera, clock.TotalSeconds % CameraPathDuration);
    scenePass.RotationRadians += delta * 0.6f;

    var context = new RenderGraphContext
    {
        DeltaSeconds = delta,
        View = camera.View,
        Projection = camera.Projection,
        ScreenWidth = (int)sceneTarget.Width,
        ScreenHeight = (int)sceneTarget.Height,
    };

    graph.Execute(gl, context);
    postChain.Apply(gl, sceneTarget, (int)sceneTarget.Width, (int)sceneTarget.Height, camera.Projection * camera.View);
    device.SwapBuffers();
});

postChain.Dispose();
sceneTarget.Dispose();
cubeVao.Dispose();
cubeVertexBuffer.Dispose();
cubeIndexBuffer.Dispose();
sceneShader.Dispose();
device.Dispose();
pluginManager.ShutdownAll();

sealed class ScenePass : RenderPass
{
    private readonly ShaderProgram _shader;
    private readonly VertexArray _vao;
    private readonly int _indexCount;

    public float RotationRadians { get; set; }

    public ScenePass(ShaderProgram shader, VertexArray vao, int indexCount)
        : base("Scene")
    {
        _shader = shader;
        _vao = vao;
        _indexCount = indexCount;
    }

    public override void Execute(GL gl, RenderGraphContext context)
    {
        gl.ClearColor(0.02f, 0.02f, 0.05f, 1f);
        gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        _shader.Use();
        var model = Matrix4x4.CreateRotationY(RotationRadians) * Matrix4x4.CreateRotationX(RotationRadians * 0.5f);
        _shader.SetUniform("uModel", model);
        _shader.SetUniform("uView", context.View);
        _shader.SetUniform("uProjection", context.Projection);

        _vao.Bind();

        unsafe
        {
            gl.DrawElements(PrimitiveType.Triangles, (uint)_indexCount, DrawElementsType.UnsignedInt, null);
        }
    }
}
