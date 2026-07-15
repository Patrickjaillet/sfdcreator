using System.Numerics;

namespace SFDCreator.Rendering.Cameras;

public sealed class OrthographicCamera : Camera
{
    public Vector2 Center { get; set; }

    public float Zoom { get; set; } = 1f;

    public float ViewportWidth { get; set; } = 1280f;

    public float ViewportHeight { get; set; } = 720f;

    public float NearPlane { get; set; } = -100f;

    public float FarPlane { get; set; } = 100f;

    public override Matrix4x4 View =>
        Matrix4x4.CreateLookAt(new Vector3(Center, 10f), new Vector3(Center, 0f), Vector3.UnitY);

    public override Matrix4x4 Projection => Matrix4x4.CreateOrthographic(
        MathF.Max(ViewportWidth / MathF.Max(Zoom, 0.0001f), 0.0001f),
        MathF.Max(ViewportHeight / MathF.Max(Zoom, 0.0001f), 0.0001f),
        NearPlane,
        FarPlane);
}
