# Changelog

All notable changes to SFD Creator are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/).

## [0.5.0] - 2026-07-15

### Added

- A new `SFDCreator.Core/Timeline` module — framework-agnostic, no OS/graphics dependency:
  - `AnimationCurve`/`FloatKeyframe`/`InterpolationMode` (Linear, Bezier via cubic Hermite tangent interpolation, and Stepped).
  - `AnimatedFloatProperty` — binds a curve to any settable float value (the "automation layer" for parameter curves).
  - `MasterTimeline`/`TimelineSegment`/`TransitionSpec` — segment sequencing with Cut and Crossfade transitions, looping.
  - `TimelinePlaybackController` — play/pause/seek/tick with a playback rate.
  - `BpmGrid` — beat-to-time/time-to-nearest-beat snapping from a manually-set BPM (no audio analysis yet — that's Phase 8).
  - `UndoRedoStack`/`ICommand`/`MoveKeyframeCommand` — command-pattern undo/redo.
- The Phase 4 `TimelinePanelContent` widget is now wired to this engine: dragging a keyframe pushes a `MoveKeyframeCommand` through the undo/redo stack, scrubbing the playhead calls `TimelinePlaybackController.Seek`, and beat-grid lines render with optional snap-while-dragging.
- `ToolbarPanelContent` gained a Play/Pause toggle button reflecting live playback state.
- The demo scene's rotation speed is now driven by a 3-keyframe Bezier `AnimationCurve` via `AnimatedFloatProperty` (the property-inspector slider still shows/moves but is overwritten by automation each frame, matching how a keyframed property behaves in a real animation tool).
- A two-segment `MasterTimeline` ("Forward" 0–6s, "Reverse" 6–12s) with a 1-second crossfade — the camera position/look-at now lerp between both segments' independently-keyframed paths during the transition window.
- Ctrl+Z / Ctrl+Y wired to `UndoRedoStack.Undo()`/`Redo()` via the existing Phase 2 `Win32InputContext` keyboard, no new input path.
- 42 new unit tests across `AnimationCurveTests`, `AnimatedFloatPropertyTests`, `MasterTimelineTests`, `TimelinePlaybackControllerTests`, `BpmGridTests`, `UndoRedoStackTests`, `MoveKeyframeCommandTests`.

### Scope notes

- BPM sync is grid-only — no audio file is loaded or analyzed; real beat detection from a waveform is Phase 8's job.
- "Automation" means parameter-curve binding, not an embedded scripting language — that's Phase 14's explicit scope.
- The timeline widget lets you retime a keyframe (drag horizontally); editing its automated *value* still requires code/the property inspector, not a curve-value-drag gesture in the timeline itself.

## [0.4.0] - 2026-07-15

### Added

- `Win32ChildPanel` (Win32 project): a custom-class child window (own WndProc, same trampoline pattern as `Win32Window`) replacing the Phase 2 `STATIC` dock panels, routing WM_PAINT/mouse/keyboard to managed code. `DockPanelHost` now creates panels from this class and gained a `Top` dock region (toolbar strip) alongside Left/Right/Bottom/Center.
- SkiaSharp UI hosting layer (`SFDCreator.UI/Hosting`): `SkiaPanelHost` renders a CPU-raster `SKSurface` per panel and blits it via GDI `SetDIBitsToDevice` on each `WM_PAINT`; `IUiPanelContent`/`UiInputState` give panel content an immediate-mode render+input contract.
- A small immediate-mode widget kit (`UiWidgets`: Button, Slider, Checkbox, Label) and a two-palette theming system (`UiTheme.Dark`/`Light`).
- Four docked panels wired into the app: a toolbar (routes through the same command handler as the native menu), a property inspector live-bound to the Phase 3 post-processing chain's tunables (bloom threshold/intensity, color-grading saturation/contrast, CRT scanline/vignette/curvature) and cube rotation speed, a node graph editor (drag nodes, drag-to-connect ports, pan/zoom), and a timeline widget (scrub playhead, drag keyframes) synced to the camera path's current time.
- Native-GL viewport overlays in `SFDCreator.Rendering` — no Skia/GDI alpha compositing involved: `GizmoPass` draws a 3D axis gizmo at the scene origin using the camera's real view/projection matrices, and `PerformanceOverlayPass` draws a live frame-time bar graph (from `FrameStats`) using an identity-MVP screen-space quad batch. Both reuse the existing `scene.vert`/`scene.frag` shader — no new GLSL files.
- Unit tests: `SliderMathTests`, `NodeGraphModelTests`, `TimelineModelTests` (new `SFDCreator.UI.Tests` project), `PerformanceGraphMathTests` (extends `SFDCreator.Rendering.Tests`), and updated `DockLayoutTests` for the new `Top` region.

### Scope notes

- The node graph editor is visual/interactive only — no compiled/executed effect semantics yet (Phase 6).
- The timeline widget is visual scrubbing/keyframe-dragging only — no playback/sync engine yet (Phase 5).
- Gizmos are rendered, not yet drag-to-transform (Phase 12's explicit scope).

## [0.3.0] - 2026-07-15

### Added

- Hand-written OpenGL device/context bootstrap (`OpenGlContextFactory`): dummy-context ARB bootstrap followed by a real modern core-profile context (4.6, falling back to 3.3) created directly against a native HWND via WGL — no `Silk.NET.Windowing` dependency.
- `IGraphicsDevice`/`GraphicsBackendKind`/`GraphicsDeviceFactory` backend-selection abstraction; only OpenGL is implemented, Vulkan/Direct3D11/Direct3D12 throw `NotSupportedException` (same pattern as the Phase 1 Naga stub).
- GL resource wrappers: `GpuBuffer`, `VertexArray`, `ShaderProgram`, `Texture2D`, `RenderTarget` (framebuffer + color/depth-stencil attachments).
- A render graph (`RenderGraph`, `RenderPass`, `RenderPassGraphSort`) with pass dependencies resolved via a unit-tested topological sort.
- A camera system: `PerspectiveCamera`/`OrthographicCamera`, and a Catmull-Rom `CameraPath` for keyframed cinematic camera moves (unit-tested).
- A post-processing chain (`PostProcessChain`): bloom (bright-pass + separable blur + composite), color grading (lift/gamma/gain + saturation/contrast), CRT scanline/vignette/curvature, and an optional camera-motion directional blur — all plain GLSL, ping-ponged through render targets.
- Frame diagnostics: `FrameClock` and a rolling-window `FrameStats` (average/min/max FPS), unit-tested.
- `DockPanelHost.PanelResized` event (Win32 project) so the renderer can resize its render targets/viewport when the center dock panel resizes.
- A procedural demo scene in `SFDCreator.App` (a rotating multi-colored cube driven by the camera path) proving the whole pipeline end-to-end, rendered into the native window's center dock panel; the render loop now runs continuously via `Win32Window.RunWithIdle` instead of the Phase 2 blocking message loop.

### Fixed

- `GetModuleHandleW` P/Invoke declarations (in both `SFDCreator.Win32` and `SFDCreator.Rendering`) were missing `CharSet = CharSet.Unicode`, causing the ANSI-marshaled module name to fail to resolve `opengl32.dll`'s handle and silently breaking legacy (GL 1.1) function loading (`glEnable` and similar) during OpenGL context bootstrap.

## [0.2.0] - 2026-07-15

### Added

- New `SFDCreator.Win32` project: a hand-written native Win32 window host (`Win32Window`) with real HWND lifecycle, WndProc, and message loop, replacing the GLFW-backed windowing from Phase 1.
- Input bridge implementing Silk.NET's `IInputContext`/`IKeyboard`/`IMouse`/`ICursor` directly from Win32 messages (`Win32InputContext`, `Win32Keyboard`, `Win32Mouse`, `Win32Cursor`), including a virtual-key-to-`Key` mapping table.
- Native menu bar support (`NativeMenuBuilder`) and a File/Help menu wired up in the app.
- Native docking-region plumbing (`DockRegion`, `DockLayout`, `DockPanelHost`) with pure, unit-tested layout math; full drag-to-redock UX is deferred to Phase 4's SkiaSharp UI shell.
- Native Open/Save file dialogs (`Win32FileDialog`) and a native message box (`NativeMessageBox`).
- Drag-and-drop support for files (`WM_DROPFILES`) surfaced as a `FilesDropped` event.
- DPI-awareness (Per-Monitor V2 via `app.manifest`, `WM_DPICHANGED` handling) and multi-monitor enumeration (`Win32Monitors`).
- Application settings persistence: `ISettingsStore`/`ApplicationSettings` abstraction in Core, with `RegistrySettingsStore` (default) and `JsonFileSettingsStore` implementations in IO. Window position/size round-trip across runs.
- Unit tests for the VK-to-`Key` mapping and dock layout math (`SFDCreator.Win32.Tests`).

### Changed

- Removed the GLFW-backed `RenderWindow`/`RenderWindowOptions` from `SFDCreator.Rendering` and the `Silk.NET.Windowing` package reference — windowing and input are now owned entirely by the native Win32 layer.

## [0.1.0] - 2026-07-15

### Added

- Solution structure: `SFDCreator.Core`, `SFDCreator.Rendering`, `SFDCreator.UI`, `SFDCreator.Audio`, `SFDCreator.IO`, `SFDCreator.Tools`, `SFDCreator.App`.
- C# 12 / .NET 8 (Windows-only) project configuration with shared build properties and automatic version serialization from `VERSION`.
- Silk.NET-based windowing, input, and OpenGL context abstraction (`RenderWindow`).
- Shader compilation abstraction (`IShaderCompiler`) with a Naga-backed implementation stub pending native interop.
- SkiaSharp-based surface host for 2D UI/overlay rendering (`SkiaSurfaceHost`).
- Plugin/module architecture (`IPlugin`, `IEffectPlugin`, `ITransitionPlugin`, `IGeneratorPlugin`, `PluginManager`) with unit tests.
- CI build pipeline (GitHub Actions) running restore/build/test on every push and pull request.
- Initial `README.md`, `LICENSE` (MIT), and this `CHANGELOG.md`.
