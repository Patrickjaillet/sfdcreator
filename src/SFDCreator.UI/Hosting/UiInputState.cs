using Silk.NET.Input;

namespace SFDCreator.UI.Hosting;

public sealed class UiInputState
{
    public float MouseX { get; internal set; }

    public float MouseY { get; internal set; }

    public bool IsLeftDown { get; internal set; }

    public bool IsRightDown { get; internal set; }

    public bool WasLeftPressedThisFrame { get; internal set; }

    public bool WasLeftReleasedThisFrame { get; internal set; }

    public float WheelDelta { get; internal set; }

    public HashSet<Key> PressedKeys { get; } = new();

    internal void ResetPerFrameFlags()
    {
        WasLeftPressedThisFrame = false;
        WasLeftReleasedThisFrame = false;
        WheelDelta = 0f;
    }
}
