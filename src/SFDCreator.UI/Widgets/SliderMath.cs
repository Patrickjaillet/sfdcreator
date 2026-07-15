namespace SFDCreator.UI.Widgets;

public static class SliderMath
{
    public static float PositionToValue(float trackLeft, float trackWidth, float min, float max, float x)
    {
        if (trackWidth <= 0f)
        {
            return min;
        }

        var t = Math.Clamp((x - trackLeft) / trackWidth, 0f, 1f);
        return min + (t * (max - min));
    }

    public static float ValueToPosition(float trackLeft, float trackWidth, float min, float max, float value)
    {
        if (max <= min)
        {
            return trackLeft;
        }

        var t = Math.Clamp((value - min) / (max - min), 0f, 1f);
        return trackLeft + (t * trackWidth);
    }
}
