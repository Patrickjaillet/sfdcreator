using System.Numerics;
using System.Runtime.InteropServices;

namespace SFDCreator.Rendering.Gizmos;

internal static class GizmoGeometry
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Color;

        public Vertex(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }

    public static Vertex[] BuildAxisLines(float length = 1.5f)
    {
        var red = new Vector3(1f, 0.2f, 0.2f);
        var green = new Vector3(0.2f, 1f, 0.2f);
        var blue = new Vector3(0.3f, 0.5f, 1f);

        return new[]
        {
            new Vertex(Vector3.Zero, red),
            new Vertex(new Vector3(length, 0f, 0f), red),

            new Vertex(Vector3.Zero, green),
            new Vertex(new Vector3(0f, length, 0f), green),

            new Vertex(Vector3.Zero, blue),
            new Vertex(new Vector3(0f, 0f, length), blue),
        };
    }
}
