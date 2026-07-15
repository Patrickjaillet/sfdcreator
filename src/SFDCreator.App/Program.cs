using System.Numerics;
using SFDCreator.App;
using SFDCreator.Core.Plugins;
using SFDCreator.Core.Settings;
using SFDCreator.Core.Timeline.Automation;
using SFDCreator.Core.Timeline.Interpolation;
using SFDCreator.Core.Timeline.Playback;
using SFDCreator.Core.Timeline.Sequencing;
using SFDCreator.Core.Timeline.Sync;
using SFDCreator.Core.Timeline.UndoRedo;
using SFDCreator.IO.Settings;
using SFDCreator.Rendering.Backend;
using SFDCreator.Rendering.Cameras;
using SFDCreator.Rendering.Diagnostics;
using SFDCreator.Rendering.Gizmos;
using SFDCreator.Rendering.Graph;
using SFDCreator.Rendering.OpenGL;
using SFDCreator.Rendering.PostProcessing;
using SFDCreator.Rendering.Resources;
using SFDCreator.UI.Hosting;
using SFDCreator.UI.Panels;
using SFDCreator.UI.Panels.NodeGraph;
using SFDCreator.UI.Panels.Timeline;
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
const int CommandPlaybackToggle = 3001;
const float CameraPathDuration = 12f;
const int RotationTrackKeyframeOffset = 3;

var rotationSpeed = 0.6f;
var playbackController = new TimelinePlaybackController();
playbackController.Play();

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

void HandleCommand(int commandId)
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

        case CommandPlaybackToggle:
            if (playbackController.IsPlaying)
            {
                playbackController.Pause();
            }
            else
            {
                playbackController.Play();
            }

            break;
    }
}

window.MenuCommand += HandleCommand;

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

var forwardCameraKeyframes = new[]
{
    new CameraPathKeyframe(0f, new Vector3(0f, 1.5f, 4f), Vector3.Zero),
    new CameraPathKeyframe(2f, new Vector3(3.5f, 1f, 0f), Vector3.Zero),
    new CameraPathKeyframe(4f, new Vector3(0f, 1.5f, -4f), Vector3.Zero),
    new CameraPathKeyframe(6f, new Vector3(-3.5f, 1f, 0f), Vector3.Zero),
};

var reverseCameraKeyframes = new[]
{
    new CameraPathKeyframe(6f, new Vector3(-3.5f, 1f, 0f), Vector3.Zero),
    new CameraPathKeyframe(8f, new Vector3(0f, 2.5f, -3f), Vector3.Zero),
    new CameraPathKeyframe(10f, new Vector3(3.5f, 1f, 0f), Vector3.Zero),
    new CameraPathKeyframe(12f, new Vector3(0f, 1.5f, 4f), Vector3.Zero),
};

var masterTimeline = new MasterTimeline();
masterTimeline.AddSegment(new TimelineSegment("Forward", 0f, 6f, TransitionSpec.Cut));
masterTimeline.AddSegment(new TimelineSegment("Reverse", 6f, 6f, new TransitionSpec(TransitionKind.Crossfade, 1f)));

var rotationCurve = new AnimationCurve();
rotationCurve.AddKeyframe(new FloatKeyframe(0f, 0.2f, InterpolationMode.Bezier, OutTangent: 0.3f));
rotationCurve.AddKeyframe(new FloatKeyframe(6f, 1.5f, InterpolationMode.Bezier, InTangent: 0.3f, OutTangent: -0.3f));
rotationCurve.AddKeyframe(new FloatKeyframe(12f, 0.2f, InterpolationMode.Bezier, InTangent: -0.3f));
var rotationAutomation = new AnimatedFloatProperty(rotationCurve, value => rotationSpeed = value);

var bpmGrid = new BpmGrid(bpm: 120f, beatsPerBar: 4);
var undoRedoStack = new UndoRedoStack();

var scenePass = new ScenePass(sceneShader, cubeVao, cubeIndexCount) { Writes = new[] { sceneTarget } };

var graph = new RenderGraph();
graph.AddPass(scenePass);

var gizmoPass = new GizmoPass(gl, shaderDirectory);
var performanceOverlayPass = new PerformanceOverlayPass(gl, shaderDirectory);

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

var toolbarContent = new ToolbarPanelContent(
    ("Open", CommandFileOpen),
    ("Save", CommandFileSave),
    ("Exit", CommandFileExit))
{
    PlaybackToggleCommandId = CommandPlaybackToggle,
    IsPlaying = () => playbackController.IsPlaying,
};
toolbarContent.CommandInvoked += HandleCommand;
using var toolbarHost = new SkiaPanelHost(dockPanels.GetPanel(DockRegion.Top), toolbarContent);

var propertyInspectorContent = new PropertyInspectorContent(
    postChain.Bloom,
    postChain.ColorGrading,
    postChain.CrtScanline,
    () => rotationSpeed,
    value => rotationSpeed = value);
using var propertyInspectorHost = new SkiaPanelHost(dockPanels.GetPanel(DockRegion.Right), propertyInspectorContent);

var nodeGraphModel = new NodeGraphModel();
var generatorNode = nodeGraphModel.AddNode("Cube Generator", new Vector2(40, 40), 0, 1);
var colorNode = nodeGraphModel.AddNode("Color", new Vector2(40, 160), 0, 1);
var compositeNode = nodeGraphModel.AddNode("Composite", new Vector2(260, 90), 2, 1);
var outputNode = nodeGraphModel.AddNode("Output", new Vector2(480, 90), 1, 0);
nodeGraphModel.Connect(generatorNode.Id, 0, compositeNode.Id, 0);
nodeGraphModel.Connect(colorNode.Id, 0, compositeNode.Id, 1);
nodeGraphModel.Connect(compositeNode.Id, 0, outputNode.Id, 0);
var nodeGraphContent = new NodeGraphPanelContent(nodeGraphModel);
using var nodeGraphHost = new SkiaPanelHost(dockPanels.GetPanel(DockRegion.Left), nodeGraphContent);

var timelineModel = new TimelineModel { DurationSeconds = CameraPathDuration };
timelineModel.AddTrack("Camera");
timelineModel.AddTrack("Rotation");
timelineModel.AddKeyframe(0, 0f);
timelineModel.AddKeyframe(0, 6f);
timelineModel.AddKeyframe(0, 12f);
foreach (var keyframe in rotationCurve.Keyframes)
{
    timelineModel.AddKeyframe(1, keyframe.Time);
}

void SyncRotationTrackVisuals()
{
    for (var i = 0; i < rotationCurve.Keyframes.Count; i++)
    {
        timelineModel.MoveKeyframe(RotationTrackKeyframeOffset + i, rotationCurve.Keyframes[i].Time);
    }
}

var timelineContent = new TimelinePanelContent(timelineModel)
{
    Grid = bpmGrid,
    SnapToGrid = bpmGrid.SnapToGrid,
};

timelineContent.KeyframeMoved += (index, time) =>
{
    if (index < RotationTrackKeyframeOffset)
    {
        return;
    }

    var curveIndex = index - RotationTrackKeyframeOffset;
    if (curveIndex < 0 || curveIndex >= rotationCurve.Keyframes.Count)
    {
        return;
    }

    var oldTime = rotationCurve.Keyframes[curveIndex].Time;
    undoRedoStack.Do(new MoveKeyframeCommand(rotationCurve, oldTime, time));
    SyncRotationTrackVisuals();
};

timelineContent.PlayheadScrubbed += time => playbackController.Seek(time);

using var timelineHost = new SkiaPanelHost(dockPanels.GetPanel(DockRegion.Bottom), timelineContent);

var keyboard = window.Input.Keyboards[0];
keyboard.KeyDown += (_, key, _) =>
{
    var ctrlHeld = keyboard.IsKeyPressed(Key.ControlLeft) || keyboard.IsKeyPressed(Key.ControlRight);
    if (!ctrlHeld)
    {
        return;
    }

    if (key == Key.Z)
    {
        undoRedoStack.Undo();
        SyncRotationTrackVisuals();
    }
    else if (key == Key.Y)
    {
        undoRedoStack.Redo();
        SyncRotationTrackVisuals();
    }
};

window.RunWithIdle(() =>
{
    device.MakeCurrent();

    var delta = clock.Tick();
    stats.Record(delta);

    playbackController.Tick(delta);
    var time = playbackController.CurrentTime;

    var blend = masterTimeline.Evaluate(time);
    var (currentPosition, currentLookAt) = blend.Current.Name == "Forward"
        ? CameraPath.Evaluate(forwardCameraKeyframes, time)
        : CameraPath.Evaluate(reverseCameraKeyframes, time);

    if (blend.Next is { } nextSegment)
    {
        var (nextPosition, nextLookAt) = nextSegment.Name == "Forward"
            ? CameraPath.Evaluate(forwardCameraKeyframes, time)
            : CameraPath.Evaluate(reverseCameraKeyframes, time);

        camera.Position = Vector3.Lerp(currentPosition, nextPosition, blend.BlendWeight);
        camera.Target = Vector3.Lerp(currentLookAt, nextLookAt, blend.BlendWeight);
    }
    else
    {
        camera.Position = currentPosition;
        camera.Target = currentLookAt;
    }

    rotationAutomation.Apply(time);
    scenePass.RotationRadians += delta * rotationSpeed;
    timelineModel.PlayheadSeconds = time;

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
    gizmoPass.Render(gl, camera.View, camera.Projection);
    performanceOverlayPass.Render(gl, stats, (int)sceneTarget.Width, (int)sceneTarget.Height);
    device.SwapBuffers();
});

gizmoPass.Dispose();
performanceOverlayPass.Dispose();
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
