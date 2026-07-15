using Silk.NET.Input;

namespace SFDCreator.Win32.Input;

public static class VirtualKeyMap
{
    private const int VK_BACK = 0x08;
    private const int VK_TAB = 0x09;
    private const int VK_RETURN = 0x0D;
    private const int VK_SHIFT = 0x10;
    private const int VK_CONTROL = 0x11;
    private const int VK_MENU = 0x12;
    private const int VK_PAUSE = 0x13;
    private const int VK_CAPITAL = 0x14;
    private const int VK_ESCAPE = 0x1B;
    private const int VK_SPACE = 0x20;
    private const int VK_PRIOR = 0x21;
    private const int VK_NEXT = 0x22;
    private const int VK_END = 0x23;
    private const int VK_HOME = 0x24;
    private const int VK_LEFT = 0x25;
    private const int VK_UP = 0x26;
    private const int VK_RIGHT = 0x27;
    private const int VK_DOWN = 0x28;
    private const int VK_SNAPSHOT = 0x2C;
    private const int VK_INSERT = 0x2D;
    private const int VK_DELETE = 0x2E;
    private const int VK_0 = 0x30;
    private const int VK_9 = 0x39;
    private const int VK_A = 0x41;
    private const int VK_Z = 0x5A;
    private const int VK_LWIN = 0x5B;
    private const int VK_RWIN = 0x5C;
    private const int VK_APPS = 0x5D;
    private const int VK_NUMPAD0 = 0x60;
    private const int VK_NUMPAD9 = 0x69;
    private const int VK_MULTIPLY = 0x6A;
    private const int VK_ADD = 0x6B;
    private const int VK_SUBTRACT = 0x6D;
    private const int VK_DECIMAL = 0x6E;
    private const int VK_DIVIDE = 0x6F;
    private const int VK_F1 = 0x70;
    private const int VK_F24 = 0x87;
    private const int VK_NUMLOCK = 0x90;
    private const int VK_SCROLL = 0x91;
    private const int VK_OEM_1 = 0xBA;
    private const int VK_OEM_PLUS = 0xBB;
    private const int VK_OEM_COMMA = 0xBC;
    private const int VK_OEM_MINUS = 0xBD;
    private const int VK_OEM_PERIOD = 0xBE;
    private const int VK_OEM_2 = 0xBF;
    private const int VK_OEM_3 = 0xC0;
    private const int VK_OEM_4 = 0xDB;
    private const int VK_OEM_5 = 0xDC;
    private const int VK_OEM_6 = 0xDD;
    private const int VK_OEM_7 = 0xDE;

    public static Key Map(int virtualKey, int scanCode, bool isExtendedKey)
    {
        if (virtualKey == VK_SHIFT)
        {
            return scanCode == 0x36 ? Key.ShiftRight : Key.ShiftLeft;
        }

        if (virtualKey == VK_CONTROL)
        {
            return isExtendedKey ? Key.ControlRight : Key.ControlLeft;
        }

        if (virtualKey == VK_MENU)
        {
            return isExtendedKey ? Key.AltRight : Key.AltLeft;
        }

        if (virtualKey is >= VK_A and <= VK_Z)
        {
            return Key.A + (virtualKey - VK_A);
        }

        if (virtualKey is >= VK_F1 and <= VK_F24)
        {
            return Key.F1 + (virtualKey - VK_F1);
        }

        if (virtualKey is >= VK_NUMPAD0 and <= VK_NUMPAD9)
        {
            return Key.Keypad0 + (virtualKey - VK_NUMPAD0);
        }

        if (virtualKey == VK_0)
        {
            return Key.Number0;
        }

        if (virtualKey is > VK_0 and <= VK_9)
        {
            return Key.Number1 + (virtualKey - VK_0 - 1);
        }

        return virtualKey switch
        {
            VK_BACK => Key.Backspace,
            VK_TAB => Key.Tab,
            VK_RETURN => Key.Enter,
            VK_PAUSE => Key.Pause,
            VK_CAPITAL => Key.CapsLock,
            VK_ESCAPE => Key.Escape,
            VK_SPACE => Key.Space,
            VK_PRIOR => Key.PageUp,
            VK_NEXT => Key.PageDown,
            VK_END => Key.End,
            VK_HOME => Key.Home,
            VK_LEFT => Key.Left,
            VK_UP => Key.Up,
            VK_RIGHT => Key.Right,
            VK_DOWN => Key.Down,
            VK_SNAPSHOT => Key.PrintScreen,
            VK_INSERT => Key.Insert,
            VK_DELETE => Key.Delete,
            VK_LWIN => Key.SuperLeft,
            VK_RWIN => Key.SuperRight,
            VK_APPS => Key.Menu,
            VK_MULTIPLY => Key.KeypadMultiply,
            VK_ADD => Key.KeypadAdd,
            VK_SUBTRACT => Key.KeypadSubtract,
            VK_DECIMAL => Key.KeypadDecimal,
            VK_DIVIDE => Key.KeypadDivide,
            VK_NUMLOCK => Key.NumLock,
            VK_SCROLL => Key.ScrollLock,
            VK_OEM_1 => Key.Semicolon,
            VK_OEM_PLUS => Key.Equal,
            VK_OEM_COMMA => Key.Comma,
            VK_OEM_MINUS => Key.Minus,
            VK_OEM_PERIOD => Key.Period,
            VK_OEM_2 => Key.Slash,
            VK_OEM_3 => Key.GraveAccent,
            VK_OEM_4 => Key.LeftBracket,
            VK_OEM_5 => Key.BackSlash,
            VK_OEM_6 => Key.RightBracket,
            VK_OEM_7 => Key.Apostrophe,
            _ => Key.Unknown,
        };
    }
}
