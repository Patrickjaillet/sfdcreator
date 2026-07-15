namespace SFDCreator.Rendering.Cameras;

public sealed class CameraPathAnimator
{
    private readonly IReadOnlyList<CameraPathKeyframe> _keyframes;

    public CameraPathAnimator(IReadOnlyList<CameraPathKeyframe> keyframes)
    {
        _keyframes = keyframes;
    }

    public void Apply(PerspectiveCamera camera, float time)
    {
        var (position, lookAt) = CameraPath.Evaluate(_keyframes, time);
        camera.Position = position;
        camera.Target = lookAt;
    }
}
