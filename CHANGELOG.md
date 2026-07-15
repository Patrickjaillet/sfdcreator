# Changelog

All notable changes to SFD Creator are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/).

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
