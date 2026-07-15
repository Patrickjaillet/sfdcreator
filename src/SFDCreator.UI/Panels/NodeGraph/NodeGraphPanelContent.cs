using System.Numerics;
using SFDCreator.UI.Hosting;
using SFDCreator.UI.Theming;
using SkiaSharp;

namespace SFDCreator.UI.Panels.NodeGraph;

public sealed class NodeGraphPanelContent : IUiPanelContent
{
    private static readonly SKFont Font = new(SKTypeface.Default, 12f);

    private readonly NodeGraphModel _model;
    private Vector2 _pan = new(40f, 40f);
    private float _zoom = 1f;
    private int? _draggingNodeId;
    private PortHandle? _connectingFrom;
    private Vector2 _lastMouseScreen;

    public NodeGraphPanelContent(NodeGraphModel model)
    {
        _model = model;
    }

    public void Render(SKCanvas canvas, SKRect bounds, UiInputState input)
    {
        var theme = UiTheme.Current;

        using var background = new SKPaint { Color = theme.Background, Style = SKPaintStyle.Fill };
        canvas.DrawRect(bounds, background);

        if (input.WheelDelta != 0f)
        {
            _zoom = Math.Clamp(_zoom + (input.WheelDelta * 0.1f), 0.5f, 2f);
        }

        var mouseScreen = new Vector2(input.MouseX, input.MouseY);
        var mouseWorld = ScreenToWorld(mouseScreen, bounds);

        HandleInteraction(input, mouseScreen, mouseWorld);

        canvas.Save();
        canvas.ClipRect(bounds);
        canvas.Translate(bounds.Left + _pan.X, bounds.Top + _pan.Y);
        canvas.Scale(_zoom);

        foreach (var edge in _model.Edges)
        {
            var from = _model.Nodes.First(n => n.Id == edge.FromNodeId).GetOutputPortPosition(edge.FromPort);
            var to = _model.Nodes.First(n => n.Id == edge.ToNodeId).GetInputPortPosition(edge.ToPort);
            DrawEdgeCurve(canvas, from, to, theme.TextMuted);
        }

        foreach (var node in _model.Nodes)
        {
            DrawNode(canvas, node, theme);
        }

        if (_connectingFrom is { } connecting)
        {
            var connectingNode = _model.Nodes.FirstOrDefault(n => n.Id == connecting.NodeId);
            if (connectingNode is not null)
            {
                var from = connecting.IsOutput
                    ? connectingNode.GetOutputPortPosition(connecting.PortIndex)
                    : connectingNode.GetInputPortPosition(connecting.PortIndex);

                DrawEdgeCurve(canvas, from, mouseWorld, theme.Accent);
            }
        }

        canvas.Restore();
    }

    private void HandleInteraction(UiInputState input, Vector2 mouseScreen, Vector2 mouseWorld)
    {
        if (input.WasLeftPressedThisFrame)
        {
            var port = _model.HitTestPort(mouseWorld);
            if (port is not null)
            {
                _connectingFrom = port;
            }
            else
            {
                _draggingNodeId = _model.HitTestNode(mouseWorld)?.Id;
            }

            _lastMouseScreen = mouseScreen;
        }
        else if (input.IsLeftDown)
        {
            var screenDelta = mouseScreen - _lastMouseScreen;

            if (_draggingNodeId is { } nodeId)
            {
                var node = _model.Nodes.FirstOrDefault(n => n.Id == nodeId);
                if (node is not null)
                {
                    node.Position += screenDelta / _zoom;
                }
            }
            else if (_connectingFrom is null)
            {
                _pan += screenDelta;
            }

            _lastMouseScreen = mouseScreen;
        }

        if (input.WasLeftReleasedThisFrame)
        {
            if (_connectingFrom is { } from)
            {
                var target = _model.HitTestPort(mouseWorld);
                if (target is { } to && to.IsOutput != from.IsOutput)
                {
                    var (output, input2) = from.IsOutput ? (from, to) : (to, from);
                    _model.Connect(output.NodeId, output.PortIndex, input2.NodeId, input2.PortIndex);
                }
            }

            _draggingNodeId = null;
            _connectingFrom = null;
        }
    }

    private Vector2 ScreenToWorld(Vector2 screen, SKRect bounds) =>
        new((screen.X - bounds.Left - _pan.X) / _zoom, (screen.Y - bounds.Top - _pan.Y) / _zoom);

    private static void DrawNode(SKCanvas canvas, NodeGraphNode node, UiTheme theme)
    {
        var rect = new SKRect(node.Position.X, node.Position.Y, node.Position.X + NodeGraphNode.Width, node.Position.Y + node.Height);
        var headerRect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + NodeGraphNode.HeaderHeight);

        using var bodyPaint = new SKPaint { IsAntialias = true, Color = theme.Panel, Style = SKPaintStyle.Fill };
        using var headerPaint = new SKPaint { IsAntialias = true, Color = theme.Accent, Style = SKPaintStyle.Fill };
        using var borderPaint = new SKPaint { IsAntialias = true, Color = theme.Border, Style = SKPaintStyle.Stroke, StrokeWidth = 1f };
        using var textPaint = new SKPaint { IsAntialias = true, Color = theme.Background };
        using var portPaint = new SKPaint { IsAntialias = true, Color = theme.Text, Style = SKPaintStyle.Fill };

        canvas.DrawRoundRect(rect, 6f, 6f, bodyPaint);
        canvas.Save();
        canvas.ClipRect(headerRect);
        canvas.DrawRoundRect(rect, 6f, 6f, headerPaint);
        canvas.Restore();
        canvas.DrawRoundRect(rect, 6f, 6f, borderPaint);

        canvas.DrawText(node.Title, headerRect.Left + 8f, headerRect.MidY + 4f, SKTextAlign.Left, Font, textPaint);

        for (var i = 0; i < node.InputPortCount; i++)
        {
            var p = node.GetInputPortPosition(i);
            canvas.DrawCircle(p.X, p.Y, 4f, portPaint);
        }

        for (var i = 0; i < node.OutputPortCount; i++)
        {
            var p = node.GetOutputPortPosition(i);
            canvas.DrawCircle(p.X, p.Y, 4f, portPaint);
        }
    }

    private static void DrawEdgeCurve(SKCanvas canvas, Vector2 from, Vector2 to, SKColor color)
    {
        using var paint = new SKPaint { IsAntialias = true, Color = color, Style = SKPaintStyle.Stroke, StrokeWidth = 2f };

        var controlOffset = Math.Max(40f, Math.Abs(to.X - from.X) * 0.5f);
        using var path = new SKPath();
        path.MoveTo(from.X, from.Y);
        path.CubicTo(from.X + controlOffset, from.Y, to.X - controlOffset, to.Y, to.X, to.Y);
        canvas.DrawPath(path, paint);
    }
}
