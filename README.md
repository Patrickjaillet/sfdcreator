# SFD Creator

A professional-grade megademo authoring tool for the demoscene, inspired by tools such as Conspiracy's apEx and Speckdrum's Hennecke.NET.

> **Status: early development (Phase 1 — Project Foundation & Architecture).** There is no usable editor yet; a screenshot will be added here once the editor UI has visible output.

## Tech stack

- **C# 12** on **.NET 8** (Windows 10/11 only)
- **Silk.NET** for windowing, input, and GPU context abstraction
- **Naga** as the planned shader translation/compilation backend (native interop pending — see [src/SFDCreator.Rendering/Shaders](src/SFDCreator.Rendering/Shaders))
- **SkiaSharp** for 2D UI and overlay rendering
- A native **Win32** control layer for windowing, docking, dialogs, and file I/O

## Solution structure

| Project | Purpose |
| --- | --- |
| `SFDCreator.Core` | Plugin/module architecture, shared services |
| `SFDCreator.Rendering` | Silk.NET windowing/GPU context, shader compilation abstraction |
| `SFDCreator.UI` | SkiaSharp-based editor UI and overlay compositing |
| `SFDCreator.Audio` | Audio playback and analysis engine |
| `SFDCreator.IO` | Asset pipeline and project file I/O |
| `SFDCreator.Tools` | Editor tooling |
| `SFDCreator.App` | Application entry point / native Win32 host |

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
