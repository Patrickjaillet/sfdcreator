using System.Numerics;

namespace SFDCreator.UI.Panels.NodeGraph;

public sealed class NodeGraphNode
{
    public const float Width = 140f;
    public const float HeaderHeight = 24f;
    public const float PortSpacing = 18f;

    public required int Id { get; init; }

    public required string Title { get; init; }

    public Vector2 Position { get; set; }

    public int InputPortCount { get; init; }

    public int OutputPortCount { get; init; }

    public float Height => HeaderHeight + (Math.Max(InputPortCount, OutputPortCount) * PortSpacing) + 8f;

    public Vector2 GetInputPortPosition(int index) =>
        new(Position.X, Position.Y + HeaderHeight + 8f + (index * PortSpacing));

    public Vector2 GetOutputPortPosition(int index) =>
        new(Position.X + Width, Position.Y + HeaderHeight + 8f + (index * PortSpacing));
}

public readonly record struct NodeGraphEdge(int FromNodeId, int FromPort, int ToNodeId, int ToPort);

public readonly record struct PortHandle(int NodeId, bool IsOutput, int PortIndex);

public sealed class NodeGraphModel
{
    private readonly List<NodeGraphNode> _nodes = new();
    private readonly List<NodeGraphEdge> _edges = new();
    private int _nextId;

    public IReadOnlyList<NodeGraphNode> Nodes => _nodes;

    public IReadOnlyList<NodeGraphEdge> Edges => _edges;

    public NodeGraphNode AddNode(string title, Vector2 position, int inputPorts, int outputPorts)
    {
        var node = new NodeGraphNode
        {
            Id = _nextId++,
            Title = title,
            Position = position,
            InputPortCount = inputPorts,
            OutputPortCount = outputPorts,
        };

        _nodes.Add(node);
        return node;
    }

    public void Connect(int fromNodeId, int fromPort, int toNodeId, int toPort)
    {
        _edges.Add(new NodeGraphEdge(fromNodeId, fromPort, toNodeId, toPort));
    }

    public NodeGraphNode? HitTestNode(Vector2 point)
    {
        for (var i = _nodes.Count - 1; i >= 0; i--)
        {
            var node = _nodes[i];
            var min = node.Position;
            var max = node.Position + new Vector2(NodeGraphNode.Width, node.Height);

            if (point.X >= min.X && point.X <= max.X && point.Y >= min.Y && point.Y <= max.Y)
            {
                return node;
            }
        }

        return null;
    }

    public PortHandle? HitTestPort(Vector2 point, float radius = 8f)
    {
        foreach (var node in _nodes)
        {
            for (var i = 0; i < node.OutputPortCount; i++)
            {
                if (Vector2.Distance(point, node.GetOutputPortPosition(i)) <= radius)
                {
                    return new PortHandle(node.Id, true, i);
                }
            }

            for (var i = 0; i < node.InputPortCount; i++)
            {
                if (Vector2.Distance(point, node.GetInputPortPosition(i)) <= radius)
                {
                    return new PortHandle(node.Id, false, i);
                }
            }
        }

        return null;
    }
}
