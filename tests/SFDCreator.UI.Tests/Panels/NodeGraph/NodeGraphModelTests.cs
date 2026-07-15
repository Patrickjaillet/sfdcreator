using System.Numerics;
using SFDCreator.UI.Panels.NodeGraph;

namespace SFDCreator.UI.Tests.Panels.NodeGraph;

public sealed class NodeGraphModelTests
{
    [Fact]
    public void AddNode_AssignsSequentialIds()
    {
        var model = new NodeGraphModel();

        var first = model.AddNode("A", Vector2.Zero, 1, 1);
        var second = model.AddNode("B", Vector2.Zero, 1, 1);

        Assert.Equal(0, first.Id);
        Assert.Equal(1, second.Id);
    }

    [Fact]
    public void HitTestNode_ReturnsNodeWhenPointIsInsideBounds()
    {
        var model = new NodeGraphModel();
        var node = model.AddNode("A", new Vector2(100, 100), 1, 1);

        var hit = model.HitTestNode(new Vector2(150, 110));

        Assert.Same(node, hit);
    }

    [Fact]
    public void HitTestNode_ReturnsNullWhenPointIsOutsideAllNodes()
    {
        var model = new NodeGraphModel();
        model.AddNode("A", new Vector2(100, 100), 1, 1);

        var hit = model.HitTestNode(new Vector2(0, 0));

        Assert.Null(hit);
    }

    [Fact]
    public void HitTestNode_PrefersTopmostNodeOnOverlap()
    {
        var model = new NodeGraphModel();
        model.AddNode("Bottom", new Vector2(0, 0), 1, 1);
        var top = model.AddNode("Top", new Vector2(0, 0), 1, 1);

        var hit = model.HitTestNode(new Vector2(10, 10));

        Assert.Same(top, hit);
    }

    [Fact]
    public void HitTestPort_FindsOutputPortWithinRadius()
    {
        var model = new NodeGraphModel();
        var node = model.AddNode("A", new Vector2(0, 0), 0, 1);
        var portPosition = node.GetOutputPortPosition(0);

        var hit = model.HitTestPort(portPosition + new Vector2(2, 0));

        Assert.NotNull(hit);
        Assert.Equal(node.Id, hit.Value.NodeId);
        Assert.True(hit.Value.IsOutput);
    }

    [Fact]
    public void HitTestPort_ReturnsNullWhenTooFarFromAnyPort()
    {
        var model = new NodeGraphModel();
        model.AddNode("A", new Vector2(0, 0), 1, 1);

        var hit = model.HitTestPort(new Vector2(1000, 1000));

        Assert.Null(hit);
    }

    [Fact]
    public void Connect_AddsEdgeToEdgesList()
    {
        var model = new NodeGraphModel();
        var a = model.AddNode("A", Vector2.Zero, 0, 1);
        var b = model.AddNode("B", Vector2.Zero, 1, 0);

        model.Connect(a.Id, 0, b.Id, 0);

        Assert.Single(model.Edges);
        Assert.Equal(new NodeGraphEdge(a.Id, 0, b.Id, 0), model.Edges[0]);
    }
}
