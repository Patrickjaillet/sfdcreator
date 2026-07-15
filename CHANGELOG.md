# Changelog

All notable changes to SFD Creator are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/).

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
