using SFDCreator.Rendering.Resources;
using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Graph;

public sealed class RenderGraph
{
    private readonly List<RenderPass> _passes = new();
    private IReadOnlyList<RenderPass>? _sortedCache;

    public void AddPass(RenderPass pass)
    {
        _passes.Add(pass);
        _sortedCache = null;
    }

    public void Execute(GL gl, RenderGraphContext context)
    {
        _sortedCache ??= Sort();

        foreach (var pass in _sortedCache)
        {
            if (pass.Writes.Count > 0)
            {
                pass.Writes[0].Bind();
                gl.Viewport(0, 0, pass.Writes[0].Width, pass.Writes[0].Height);
            }
            else
            {
                RenderTarget.BindDefault(gl);
                gl.Viewport(0, 0, (uint)context.ScreenWidth, (uint)context.ScreenHeight);
            }

            pass.Execute(gl, context);
        }
    }

    private IReadOnlyList<RenderPass> Sort()
    {
        var nodeToPass = new Dictionary<PassNode, RenderPass>();
        var nodes = new List<PassNode>();

        foreach (var pass in _passes)
        {
            var node = new PassNode(pass.Name, pass.Reads.Cast<object>().ToArray(), pass.Writes.Cast<object>().ToArray());
            nodes.Add(node);
            nodeToPass[node] = pass;
        }

        var orderedNodes = RenderPassGraphSort.Sort(nodes);
        return orderedNodes.Select(node => nodeToPass[node]).ToList();
    }
}
