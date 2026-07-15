using System.Numerics;

namespace SFDCreator.Rendering.Cameras;

public sealed class PerspectiveCamera : Camera
{
    public Vector3 Position { get; set; }

    public Vector3 Target { get; set; } = Vector3.Zero;

    public Vector3 Up { get; set; } = Vector3.UnitY;

    public float FieldOfViewRadians { get; set; } = MathF.PI / 4f;

    public float AspectRatio { get; set; } = 16f / 9f;

    public float NearPlane { get; set; } = 0.1f;

    public float FarPlane { get; set; } = 100f;

    public override Matrix4x4 View => Matrix4x4.CreateLookAt(Position, Target, Up);

    public override Matrix4x4 Projection =>
        Matrix4x4.CreatePerspectiveFieldOfView(FieldOfViewRadians, AspectRatio, NearPlane, FarPlane);
}
