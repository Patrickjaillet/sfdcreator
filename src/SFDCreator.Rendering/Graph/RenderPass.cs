using SFDCreator.Rendering.Resources;
using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Graph;

public abstract class RenderPass
{
    protected RenderPass(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<RenderTarget> Reads { get; init; } = Array.Empty<RenderTarget>();

    public IReadOnlyList<RenderTarget> Writes { get; init; } = Array.Empty<RenderTarget>();

    public abstract void Execute(GL gl, RenderGraphContext context);
}
