using SFDCreator.UI.Widgets;

namespace SFDCreator.UI.Tests.Widgets;

public sealed class SliderMathTests
{
    [Fact]
    public void PositionToValue_AtTrackStart_ReturnsMin()
    {
        var value = SliderMath.PositionToValue(trackLeft: 100f, trackWidth: 200f, min: 0f, max: 10f, x: 100f);

        Assert.Equal(0f, value, 3);
    }

    [Fact]
    public void PositionToValue_AtTrackEnd_ReturnsMax()
    {
        var value = SliderMath.PositionToValue(trackLeft: 100f, trackWidth: 200f, min: 0f, max: 10f, x: 300f);

        Assert.Equal(10f, value, 3);
    }

    [Fact]
    public void PositionToValue_ClampsBeyondTrackBounds()
    {
        Assert.Equal(0f, SliderMath.PositionToValue(0f, 100f, 0f, 10f, -50f), 3);
        Assert.Equal(10f, SliderMath.PositionToValue(0f, 100f, 0f, 10f, 500f), 3);
    }

    [Fact]
    public void ValueToPosition_IsInverseOfPositionToValue()
    {
        const float trackLeft = 20f;
        const float trackWidth = 180f;
        const float min = -5f;
        const float max = 5f;

        var position = SliderMath.ValueToPosition(trackLeft, trackWidth, min, max, 2.5f);
        var value = SliderMath.PositionToValue(trackLeft, trackWidth, min, max, position);

        Assert.Equal(2.5f, value, 2);
    }

    [Fact]
    public void PositionToValue_WithZeroWidthTrack_ReturnsMin()
    {
        Assert.Equal(5f, SliderMath.PositionToValue(0f, 0f, 5f, 15f, 50f), 3);
    }
}
