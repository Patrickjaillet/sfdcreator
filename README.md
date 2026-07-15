# SFD Creator

A professional-grade megademo authoring tool for the demoscene, inspired by tools such as Conspiracy's apEx and Speckdrum's Hennecke.NET.

> **Status: early development (Phase 4 — SkiaSharp UI & Overlay Compositing).** The app now has a docked editor shell (toolbar, node graph, property inspector, timeline) around a live 3D viewport (rotating cube, keyframed camera path, bloom/color-grading/CRT post-processing, axis gizmo and a frame-time graph overlay). A screenshot will be added here once the UI is visually validated.

## Tech stack

- **C# 12** on **.NET 8** (Windows 10/11 only)
- A hand-written native **Win32** window host (real HWND, own WndProc/message loop) — see [src/SFDCreator.Win32](src/SFDCreator.Win32) — with native menu bar, docking-region plumbing (now five regions: Top/Left/Right/Bottom/Center), Open/Save dialogs, drag-and-drop, DPI/multi-monitor awareness, and an input bridge implementing Silk.NET's `IInputContext`/`IKeyboard`/`IMouse`
- A hand-written **OpenGL** rendering core (own WGL context bootstrap, no `Silk.NET.Windowing`) — see [src/SFDCreator.Rendering](src/SFDCreator.Rendering) — with a render graph, render targets/framebuffers, a bloom/color-grading/CRT-scanline/motion-blur post-processing chain, a camera system with spline-based cinematic camera paths, and native-GL viewport overlays (axis gizmo, frame-time graph). Backend selection (`GraphicsBackendKind`) is abstracted, but only OpenGL is implemented — Vulkan/Direct3D11/Direct3D12 are unimplemented stubs for now.
- **SkiaSharp** (CPU raster, blitted via GDI `SetDIBitsToDevice`) for the docked editor UI — see [src/SFDCreator.UI](src/SFDCreator.UI) — a small immediate-mode widget kit (buttons/sliders/checkboxes), a property inspector live-bound to the renderer's post-processing parameters, a node graph editor, and a timeline widget, all themeable (Dark/Light)
- **Silk.NET** for GPU API bindings (`Silk.NET.OpenGL`) and input type definitions (`Key`, `MouseButton`)
- **Naga** as the planned shader translation/compilation backend (native interop still pending — see [src/SFDCreator.Rendering/Shaders](src/SFDCreator.Rendering/Shaders); shaders are authored directly in GLSL for now)

## Solution structure

| Project | Purpose |
| --- | --- |
| `SFDCreator.Core` | Plugin/module architecture, shared services, settings abstraction |
| `SFDCreator.Win32` | Native Win32 window host, input bridge, menu/dialogs/drag-drop, DPI/monitor awareness, dock panel hosting |
| `SFDCreator.Rendering` | OpenGL device/context bootstrap, render graph, render targets, post-processing, cameras, viewport overlays, shader compilation abstraction |
| `SFDCreator.UI` | SkiaSharp-based editor UI shell: panel hosting, widgets, theming, property inspector, node graph, timeline |
| `SFDCreator.Audio` | Audio playback and analysis engine |
| `SFDCreator.IO` | Asset pipeline, project file I/O, settings persistence (registry / JSON) |
| `SFDCreator.Tools` | Editor tooling |
| `SFDCreator.App` | Composition root — wires the native Win32 host, docked UI panels, and the OpenGL render loop into a running application |

## Building

Requires the .NET 8 SDK (or newer, targeting `net8.0-windows`) on Windows 10/11.

```
dotnet build
dotnet test
```

## Versioning

The product version is tracked in the [VERSION](VERSION) file and applied to every assembly via [Directory.Build.props](Directory.Build.props). Bump it with:

```
scripts/bump-version.ps1 -Part patch
```

## License

MIT — see [LICENSE](LICENSE).
