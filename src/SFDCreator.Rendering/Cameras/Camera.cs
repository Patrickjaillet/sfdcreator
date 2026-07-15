using System.Numerics;

namespace SFDCreator.Rendering.Cameras;

public abstract class Camera
{
    public abstract Matrix4x4 View { get; }

    public abstract Matrix4x4 Projection { get; }
}
