using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.Input;
using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Input;

public sealed class Win32Keyboard : IKeyboard
{
    private static readonly Key[] AllKeys = Enum.GetValues<Key>().Where(k => k != Key.Unknown).ToArray();

    private readonly HashSet<Key> _pressedKeys = new();
    private readonly HashSet<int> _pressedScancodes = new();

    public string Name => "Win32 Keyboard";

    public int Index => 0;

    public bool IsConnected => true;

    public IReadOnlyList<Key> SupportedKeys => AllKeys;

    public string ClipboardText
    {
        get
        {
            if (!User32.OpenClipboard(0))
            {
                return string.Empty;
            }

            try
            {
                var handle = User32.GetClipboardData(NativeConstants.CF_UNICODETEXT);
                if (handle == 0)
                {
                    return string.Empty;
                }

                var ptr = Kernel32.GlobalLock(handle);
                if (ptr == 0)
                {
                    return string.Empty;
                }

                try
                {
                    return Marshal.PtrToStringUni(ptr) ?? string.Empty;
                }
                finally
                {
                    Kernel32.GlobalUnlock(handle);
                }
            }
            finally
            {
                User32.CloseClipboard();
            }
        }
        set
        {
            var bytes = Encoding.Unicode.GetBytes((value ?? string.Empty) + "\0");

            if (!User32.OpenClipboard(0))
            {
                return;
            }

            try
            {
                User32.EmptyClipboard();
                var hMem = Kernel32.GlobalAlloc(Kernel32.GMEM_MOVEABLE, (nuint)bytes.Length);
                if (hMem == 0)
                {
                    return;
                }

                var ptr = Kernel32.GlobalLock(hMem);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                Kernel32.GlobalUnlock(hMem);
                User32.SetClipboardData(NativeConstants.CF_UNICODETEXT, hMem);
            }
            finally
            {
                User32.CloseClipboard();
            }
        }
    }

    public event Action<IKeyboard, Key, int>? KeyDown;

    public event Action<IKeyboard, Key, int>? KeyUp;

    public event Action<IKeyboard, char>? KeyChar;

    public bool IsKeyPressed(Key key) => _pressedKeys.Contains(key);

    public bool IsScancodePressed(int scancode) => _pressedScancodes.Contains(scancode);

    public void BeginInput()
    {
    }

    public void EndInput()
    {
    }

    internal void HandleMessage(uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case NativeConstants.WM_KEYDOWN:
            case NativeConstants.WM_SYSKEYDOWN:
            {
                var (key, scancode) = Decode(wParam, lParam);
                _pressedKeys.Add(key);
                _pressedScancodes.Add(scancode);
                KeyDown?.Invoke(this, key, scancode);
                break;
            }

            case NativeConstants.WM_KEYUP:
            case NativeConstants.WM_SYSKEYUP:
            {
                var (key, scancode) = Decode(wParam, lParam);
                _pressedKeys.Remove(key);
                _pressedScancodes.Remove(scancode);
                KeyUp?.Invoke(this, key, scancode);
                break;
            }

            case NativeConstants.WM_CHAR:
                KeyChar?.Invoke(this, (char)wParam);
                break;
        }
    }

    private static (Key Key, int Scancode) Decode(nint wParam, nint lParam)
    {
        var vk = (int)(wParam.ToInt64() & 0xFF);
        var lParamBits = lParam.ToInt64();
        var scancode = (int)((lParamBits >> 16) & 0xFF);
        var extended = (lParamBits & NativeConstants.KEY_EXTENDED_FLAG) != 0;
        return (VirtualKeyMap.Map(vk, scancode, extended), scancode);
    }
}
