using Silk.NET.Input;

namespace SFDCreator.Win32.Input;

public sealed class Win32InputContext : IInputContext
{
    private readonly Win32Keyboard _keyboard = new();
    private readonly Win32Mouse _mouse;
    private readonly IReadOnlyList<IKeyboard> _keyboards;
    private readonly IReadOnlyList<IMouse> _mice;

    public Win32InputContext(nint hwnd)
    {
        Handle = hwnd;
        _mouse = new Win32Mouse(hwnd);
        _keyboards = new IKeyboard[] { _keyboard };
        _mice = new IMouse[] { _mouse };
    }

    public nint Handle { get; }

    public IReadOnlyList<IGamepad> Gamepads { get; } = Array.Empty<IGamepad>();

    public IReadOnlyList<IJoystick> Joysticks { get; } = Array.Empty<IJoystick>();

    public IReadOnlyList<IKeyboard> Keyboards => _keyboards;

    public IReadOnlyList<IMouse> Mice => _mice;

    public IReadOnlyList<IInputDevice> OtherDevices { get; } = Array.Empty<IInputDevice>();

    public event Action<IInputDevice, bool>? ConnectionChanged;

    internal void HandleKeyboardMessage(uint msg, nint wParam, nint lParam) => _keyboard.HandleMessage(msg, wParam, lParam);

    internal void HandleMouseMessage(uint msg, nint wParam, nint lParam) => _mouse.HandleMessage(msg, wParam, lParam);

    public void Dispose()
    {
    }
}
