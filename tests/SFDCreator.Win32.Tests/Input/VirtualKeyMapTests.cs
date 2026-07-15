using SFDCreator.Win32.Input;
using Silk.NET.Input;

namespace SFDCreator.Win32.Tests.Input;

public sealed class VirtualKeyMapTests
{
    [Theory]
    [InlineData(0x41, Key.A)]
    [InlineData(0x5A, Key.Z)]
    [InlineData(0x30, Key.Number0)]
    [InlineData(0x31, Key.Number1)]
    [InlineData(0x39, Key.Number9)]
    [InlineData(0x70, Key.F1)]
    [InlineData(0x87, Key.F24)]
    [InlineData(0x60, Key.Keypad0)]
    [InlineData(0x69, Key.Keypad9)]
    [InlineData(0x0D, Key.Enter)]
    [InlineData(0x1B, Key.Escape)]
    [InlineData(0x20, Key.Space)]
    [InlineData(0x25, Key.Left)]
    [InlineData(0x5B, Key.SuperLeft)]
    [InlineData(0x5C, Key.SuperRight)]
    public void Map_TranslatesKnownVirtualKeys(int virtualKey, Key expected)
    {
        Assert.Equal(expected, VirtualKeyMap.Map(virtualKey, scanCode: 0, isExtendedKey: false));
    }

    [Fact]
    public void Map_DistinguishesLeftAndRightShiftByScancode()
    {
        Assert.Equal(Key.ShiftLeft, VirtualKeyMap.Map(0x10, scanCode: 0x2A, isExtendedKey: false));
        Assert.Equal(Key.ShiftRight, VirtualKeyMap.Map(0x10, scanCode: 0x36, isExtendedKey: false));
    }

    [Fact]
    public void Map_DistinguishesLeftAndRightControlByExtendedFlag()
    {
        Assert.Equal(Key.ControlLeft, VirtualKeyMap.Map(0x11, scanCode: 0, isExtendedKey: false));
        Assert.Equal(Key.ControlRight, VirtualKeyMap.Map(0x11, scanCode: 0, isExtendedKey: true));
    }

    [Fact]
    public void Map_DistinguishesLeftAndRightAltByExtendedFlag()
    {
        Assert.Equal(Key.AltLeft, VirtualKeyMap.Map(0x12, scanCode: 0, isExtendedKey: false));
        Assert.Equal(Key.AltRight, VirtualKeyMap.Map(0x12, scanCode: 0, isExtendedKey: true));
    }

    [Fact]
    public void Map_ReturnsUnknownForUnmappedVirtualKey()
    {
        Assert.Equal(Key.Unknown, VirtualKeyMap.Map(0xFF, scanCode: 0, isExtendedKey: false));
    }
}
