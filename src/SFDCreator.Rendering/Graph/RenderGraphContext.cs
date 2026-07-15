using System.Numerics;

namespace SFDCreator.Rendering.Graph;

public sealed class RenderGraphContext
{
    public required float DeltaSeconds { get; init; }

    public required Matrix4x4 View { get; init; }

    public required Matrix4x4 Projection { get; init; }

    public required int ScreenWidth { get; init; }

    public required int ScreenHeight { get; init; }
}
