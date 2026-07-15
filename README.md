# SFD Creator

A professional-grade megademo authoring tool for the demoscene, inspired by tools such as Conspiracy's apEx and Speckdrum's Hennecke.NET.

> **Status: early development (Phase 2 — Native Win32 Control Layer).** There is no usable editor yet; a screenshot will be added here once the editor UI has visible output.

## Tech stack

- **C# 12** on **.NET 8** (Windows 10/11 only)
- A hand-written native **Win32** window host (real HWND, own WndProc/message loop) — see [src/SFDCreator.Win32](src/SFDCreator.Win32) — with native menu bar, docking-region plumbing, Open/Save dialogs, drag-and-drop, DPI/multi-monitor awareness, and an input bridge implementing Silk.NET's `IInputContext`/`IKeyboard`/`IMouse`
- **Silk.NET** for GPU context abstraction and input type definitions (`Key`, `MouseButton`)
- **Naga** as the planned shader translation/compilation backend (native interop pending — see [src/SFDCreator.Rendering/Shaders](src/SFDCreator.Rendering/Shaders))
- **SkiaSharp** for 2D UI and overlay rendering

## Solution structure

| Project | Purpose |
| --- | --- |
| `SFDCreator.Core` | Plugin/module architecture, shared services, settings abstraction |
| `SFDCreator.Win32` | Native Win32 window host, input bridge, menu/dialogs/drag-drop, DPI/monitor awareness |
| `SFDCreator.Rendering` | GPU context abstraction, shader compilation abstraction |
| `SFDCreator.UI` | SkiaSharp-based editor UI and overlay compositing |
| `SFDCreator.Audio` | Audio playback and analysis engine |
| `SFDCreator.IO` | Asset pipeline, project file I/O, settings persistence (registry / JSON) |
| `SFDCreator.Tools` | Editor tooling |
| `SFDCreator.App` | Composition root — wires the native Win32 host into a running application |

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
