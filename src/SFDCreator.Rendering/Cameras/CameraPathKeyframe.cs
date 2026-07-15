using System.Numerics;

namespace SFDCreator.Rendering.Cameras;

public readonly record struct CameraPathKeyframe(float Time, Vector3 Position, Vector3 LookAt);
