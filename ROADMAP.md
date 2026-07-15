# ROADMAP.md — SFD Creator

**SFD Creator** is a professional-grade megademo authoring tool for the demoscene, inspired by tools such as Conspiracy's apEx and Speckdrum's Hennecke.NET. Target stack: **C# 12**, **Silk.NET**, **Naga** (shader/GPU abstraction), **SkiaSharp**, coupled with a native **Win32** control layer. Target quality bar: a commercial-grade product valued at **€2000**.

---

## License and Copyright

**SFD Creator**
Copyright © 2026 **SANDEFJORD DEVELOPMENT** — All rights reserved
Creator: Patrick JAILLET
Email: contact.shaderstudio@gmail.com
Website: https://github.com/Patrickjaillet
Official repository: https://github.com/Patrickjaillet/sfdcreator

---

## Development Conventions

- [ ] Source language entirely in **English** (variable, function, and class names)
- [ ] **No comments** in the source code
- [ ] Strict **Windows 10/11** compatibility only
- [ ] Every added feature must be reflected in this ROADMAP.md
- [ ] Automatic software version serialization at every stage
- [ ] Every change must be reflected for the end user in CHANGELOG.md
- [ ] README.md must be created and kept up to date for the end user with every change, including a screenshot of the software
- [ ] Systematic synchronization with the repository **https://github.com/Patrickjaillet/sfdcreator** on every project change
- [ ] Creation of all files and documents required for the GitHub repository
- [ ] MIT License
- [ ] Je ne veux strictement pas de "claude AI" comme contributeur dans le repositorie, son nom ne doit apparaitre nulle part
- [ ] Le ROADMAP.md ne doit pas etre dans le depot github

---

## Phase 1 — Project Foundation & Architecture

- [ ] Define solution structure (Core, Rendering, UI, Audio, IO, Tools)
- [ ] Set up C# 12 / .NET project targeting Windows 10/11 only
- [ ] Integrate Silk.NET (windowing, input, GPU context abstraction)
- [ ] Integrate Naga as shader translation/compilation backend
- [ ] Integrate SkiaSharp for 2D UI and overlay rendering
- [ ] Design plugin/module architecture (effects, transitions, generators)
- [ ] Set up CI build pipeline and automatic version serialization
- [ ] Initialize Git repository synced to GitHub (sfdcreator)
- [ ] Create initial README.md, CHANGELOG.md, LICENSE (MIT)

## Phase 2 — Native Win32 Control Layer

- [ ] Implement native Win32 window host (HWND lifecycle, message loop)
- [ ] Bridge Win32 messages to Silk.NET input/event system
- [ ] Implement native menu bar, docking panels, and dialog boxes
- [ ] Implement multi-monitor and DPI-awareness support
- [ ] Implement native file dialogs (Open/Save) via Win32 API
- [ ] Implement drag-and-drop support (assets, project files)
- [ ] Implement application settings persistence (registry / config file)

## Phase 3 — Rendering Core

- [ ] Set up GPU device/context abstraction via Silk.NET (Vulkan/D3D/OpenGL backend selection)
- [ ] Implement Naga-based shader pipeline (cross-compilation, hot-reload)
- [ ] Implement render graph / frame graph architecture
- [ ] Implement render targets, framebuffers, and multi-pass rendering
- [ ] Implement post-processing pipeline (bloom, motion blur, color grading, CRT/scanline)
- [ ] Implement camera system (2D/3D, path-based cinematic cameras)
- [ ] Benchmark and optimize rendering for 60/120 fps demo playback

## Phase 4 — SkiaSharp UI & Overlay Compositing

- [ ] Build the main editor UI shell (docking, panels, toolbars) with SkiaSharp
- [ ] Implement property inspector / parameter editors (curves, colors, vectors)
- [ ] Implement node-based effect graph editor
- [ ] Implement timeline UI widget (tracks, keyframes, markers)
- [ ] Implement live preview viewport with overlay gizmos
- [ ] Implement themeable UI skinning system
- [ ] Implement UI performance profiling overlay (fps, GPU/CPU timings)

## Phase 5 — Timeline & Synchronization Engine

- [ ] Implement master timeline with sub-scene/segment sequencing
- [ ] Implement keyframe interpolation system (linear, bezier, stepped)
- [ ] Implement music-to-visual sync markers (BPM grid, beat detection)
- [ ] Implement scripting/automation layer for parameter curves
- [ ] Implement scene transition system (cuts, crossfades, custom transitions)
- [ ] Implement real-time scrubbing and frame-accurate playback
- [ ] Implement undo/redo system across the whole timeline

## Phase 6 — Effects & Shader Engine

- [ ] Implement shader library (raymarching, particle systems, fractals, plasma, tunnels)
- [ ] Implement GPU particle system (compute-based, millions of particles)
- [ ] Implement procedural texture/noise generation tools
- [ ] Implement classic demoscene effect presets (rotozoomers, metaballs, voxel landscapes)
- [ ] Implement custom shader editor with live compilation via Naga
- [ ] Implement effect parameter automation binding to the timeline
- [ ] Implement effect performance profiler and shader validator

## Phase 7 — Asset Pipeline & Content Management

- [ ] Implement asset importer (models, textures, fonts, videos)
- [ ] Implement asset library/browser panel with thumbnails
- [ ] Implement 3D model support (glTF/FBX loading, skinning, animation)
- [ ] Implement font rendering pipeline (vector fonts, demoscene bitmap fonts)
- [ ] Implement project packaging format (.sfdproj) with dependency tracking
- [ ] Implement asset compression and streaming for large productions
- [ ] Implement version control friendly project file format

## Phase 8 — Audio Engine & Music Synchronization

- [ ] Integrate audio playback engine (WAV/OGG/tracker module formats)
- [ ] Implement real-time audio analysis (FFT, beat/onset detection)
- [ ] Implement audio-reactive parameter binding for effects
- [ ] Implement soundtrack synchronization tools (waveform view, BPM tap)
- [ ] Implement audio mixing and mastering-lite tools (volume automation, fades)
- [ ] Implement support for tracker music formats common in the demoscene

## Phase 9 — Export, Packaging & Distribution

- [ ] Implement standalone executable export (single-file, self-contained)
- [ ] Implement demo runtime optimized for size (4k/64k intro constraints support)
- [ ] Implement video capture/export (lossless, high-res, high-fps)
- [ ] Implement compo-ready packaging (README, screenshots, executable signing)
- [ ] Implement installer builder for end users
- [ ] Implement crash reporting and diagnostics for released demos
- [ ] Implement automatic CHANGELOG.md and version bump on each release

## Phase 10 — Polish, QA, Documentation & GitHub Repository

- [ ] Full regression test pass across Windows 10/11 target machines
- [ ] Performance profiling and optimization pass (rendering, audio, UI)
- [ ] Write full end-user documentation and tutorials
- [ ] Capture and update README.md screenshot with each significant UI change
- [ ] Finalize CHANGELOG.md for the release
- [ ] Finalize LICENSE (MIT) and third-party license attributions
- [ ] Publish/sync final state to https://github.com/Patrickjaillet/sfdcreator
- [ ] Prepare marketing/showcase materials reflecting the €2000 professional positioning

## Phase 11 — Advanced Rendering & Visual Fidelity

- [ ] Implement HDR rendering pipeline and tone mapping (ACES, Reinhard)
- [ ] Implement volumetric lighting and fog effects
- [ ] Implement screen-space reflections and ambient occlusion (SSAO/SSGI)
- [ ] Implement temporal anti-aliasing (TAA) and upscaling (FSR-like integration)
- [ ] Implement physically based rendering (PBR) material system
- [ ] Implement dynamic shadow mapping (cascaded shadow maps)
- [ ] Implement GPU-driven rendering (indirect draw calls, instancing at scale)
- [ ] Implement render pipeline presets for different demo styles (retro/CRT vs modern PBR)

## Phase 12 — Scene Graph & 3D Content Tools

- [ ] Implement full scene graph (hierarchical transforms, parenting, groups)
- [ ] Implement 3D object manipulation gizmos (move/rotate/scale) in viewport
- [ ] Implement skeletal animation and blend-tree system
- [ ] Implement camera path editor with spline-based motion
- [ ] Implement terrain/voxel landscape editor
- [ ] Implement light placement and light probe baking tools
- [ ] Implement scene instancing and prefab/template system

## Phase 13 — Node-Based Compositing & Post-Production

- [ ] Extend node graph editor to full compositing pipeline (layers, blend modes)
- [ ] Implement masking and rotoscoping tools for overlays
- [ ] Implement color grading node suite (curves, LUTs, split-toning)
- [ ] Implement chromatic aberration, film grain, vignette, lens distortion nodes
- [ ] Implement text/typography compositing tools (kinetic typography for credits)
- [ ] Implement export of compositing presets/templates for reuse across productions
- [ ] Implement real-time preview of the full composite chain at final resolution

## Phase 14 — Scripting & Extensibility

- [ ] Design and implement a scripting API (C# scripting or embedded language)
- [ ] Implement hot-reloadable user scripts for custom effect behavior
- [ ] Implement plugin SDK and plugin discovery/loading system
- [ ] Implement sandboxing/safety checks for third-party plugins
- [ ] Implement scripting console/debugger inside the editor
- [ ] Document scripting API for community/third-party demogroup usage
- [ ] Provide example plugins (custom effect, custom importer, custom exporter)

## Phase 15 — Collaboration & Project Management

- [ ] Implement project templates for common demo formats (64k, 4k, full demo, invitro)
- [ ] Implement multi-user friendly project file diffing (merge-safe format)
- [ ] Implement asset locking/checkout indicators for team workflows
- [ ] Implement in-app task/TODO tracking tied to timeline segments
- [ ] Implement project versioning/snapshot system (local history)
- [ ] Implement export of production notes for demogroup credits/greetings
- [ ] Implement integration hooks for external version control (Git status display)

## Phase 16 — Performance Engineering & Size Optimization

- [ ] Implement shader/code minification pipeline for 4k/64k intros
- [ ] Implement asset compression profiles (texture, audio, geometry)
- [ ] Implement procedural content generation tools to reduce asset size
- [ ] Implement executable packer/compressor integration (Crinkler-style workflow)
- [ ] Implement runtime memory profiler and leak detector
- [ ] Implement GPU frame timing analyzer with bottleneck detection
- [ ] Implement automated size-budget reporting per production target

## Phase 17 — Hardware Compatibility & Driver Layer

- [ ] Implement GPU capability detection and feature-level fallback paths
- [ ] Implement multi-backend support validation (Vulkan/D3D11/D3D12 via Silk.NET)
- [ ] Implement compatibility testing matrix (GPU vendors: NVIDIA/AMD/Intel)
- [ ] Implement graceful degradation for older Windows 10 hardware
- [ ] Implement driver crash recovery and safe-mode rendering fallback
- [ ] Implement diagnostic report generator for support/bug reports

## Phase 18 — Live Performance & Playback Mode

- [ ] Implement standalone "player mode" for live demoparty execution
- [ ] Implement external sync input (MIDI clock, timecode, DMX for stage shows)
- [ ] Implement fail-safe playback (auto-recovery on frame drop/crash)
- [ ] Implement dual-output support (preview screen + main projection screen)
- [ ] Implement remote control panel (tablet/second PC) for live show operation
- [ ] Implement latency-optimized audio-video sync for live playback
- [ ] Implement kiosk/fullscreen exclusive mode with input lockout

## Phase 19 — Community, Licensing & Monetization Infrastructure

- [ ] Implement license key activation system consistent with €2000 commercial positioning
- [ ] Implement trial/demo mode with feature limitations
- [ ] Implement update-checker and in-app patch notes viewer
- [ ] Implement telemetry (opt-in) for crash/usage analytics
- [ ] Implement user forum/community links and in-app feedback submission
- [ ] Prepare EULA and commercial licensing documents (separate from MIT engine core, if applicable)
- [ ] Set up sales/distribution page and payment integration references

## Phase 20 — Final Release, Long-Term Support & Roadmap Closure

- [ ] Conduct final full-scope QA across all modules (rendering, audio, UI, export)
- [ ] Conduct external beta test with demoscene groups (apEx/Hennecke.NET-style users)
- [ ] Incorporate beta feedback into final polish pass
- [ ] Finalize v1.0 release build with automatic version serialization
- [ ] Publish final README.md, CHANGELOG.md, and screenshots to GitHub repository
- [ ] Tag v1.0 release on https://github.com/Patrickjaillet/sfdcreator
- [ ] Define post-1.0 maintenance and long-term support (LTS) policy
- [ ] Draft roadmap for v2.0 (community-requested features, new effect modules)
