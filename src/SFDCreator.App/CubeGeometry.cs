using System.Numerics;
using System.Runtime.InteropServices;
using SFDCreator.Rendering.Resources;
using Silk.NET.OpenGL;

namespace SFDCreator.App;

internal static class CubeGeometry
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

    public static (GpuBuffer VertexBuffer, GpuBuffer IndexBuffer, int IndexCount) Create(GL gl)
    {
        var white = new Vector3(2f, 2f, 2f);
        var red = new Vector3(0.8f, 0.15f, 0.15f);
        var green = new Vector3(0.15f, 0.7f, 0.2f);
        var blue = new Vector3(0.15f, 0.35f, 0.85f);
        var amber = new Vector3(0.85f, 0.6f, 0.1f);
        var purple = new Vector3(0.5f, 0.2f, 0.7f);

        var faceVertices = new[]
        {
            new[]
            {
                new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f),
            },
            new[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
            },
            new[]
            {
                new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f),
            },
            new[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f),
            },
            new[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
            },
            new[]
            {
                new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
            },
        };

        var faceColors = new[] { red, amber, green, purple, white, blue };

        var vertices = new List<Vertex>();
        var indices = new List<uint>();

        for (var face = 0; face < faceVertices.Length; face++)
        {
            var baseIndex = (uint)vertices.Count;

            foreach (var position in faceVertices[face])
            {
                vertices.Add(new Vertex(position, faceColors[face]));
            }

            indices.Add(baseIndex + 0);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 0);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 3);
        }

        var vertexBuffer = new GpuBuffer(gl, BufferTargetARB.ArrayBuffer);
        vertexBuffer.Upload<Vertex>(vertices.ToArray());

        var indexBuffer = new GpuBuffer(gl, BufferTargetARB.ElementArrayBuffer);
        indexBuffer.Upload<uint>(indices.ToArray());

        return (vertexBuffer, indexBuffer, indices.Count);
    }
}
