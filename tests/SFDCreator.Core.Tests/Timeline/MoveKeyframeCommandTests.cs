using SFDCreator.Core.Timeline.Interpolation;
using SFDCreator.Core.Timeline.UndoRedo;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class MoveKeyframeCommandTests
{
    [Fact]
    public void Execute_MovesKeyframeToNewTime()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(2f, 5f));

        var command = new MoveKeyframeCommand(curve, oldTime: 2f, newTime: 8f);
        command.Execute();

        Assert.Equal(8f, curve.Keyframes[0].Time);
        Assert.Equal(5f, curve.Keyframes[0].Value);
    }

    [Fact]
    public void Undo_RestoresOriginalTime()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(2f, 5f));

        var command = new MoveKeyframeCommand(curve, oldTime: 2f, newTime: 8f);
        command.Execute();
        command.Undo();

        Assert.Equal(2f, curve.Keyframes[0].Time);
    }

    [Fact]
    public void Execute_ThenUndo_ThenExecuteAgain_RoundTrips()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 1f));
        curve.AddKeyframe(new FloatKeyframe(5f, 2f));

        var command = new MoveKeyframeCommand(curve, oldTime: 5f, newTime: 9f);

        command.Execute();
        Assert.Contains(curve.Keyframes, k => k.Time == 9f);

        command.Undo();
        Assert.Contains(curve.Keyframes, k => k.Time == 5f);

        command.Execute();
        Assert.Contains(curve.Keyframes, k => k.Time == 9f);
    }

    [Fact]
    public void UndoRedoStack_IntegratesWithMoveKeyframeCommand()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 1f));

        var stack = new UndoRedoStack();
        stack.Do(new MoveKeyframeCommand(curve, oldTime: 0f, newTime: 3f));

        Assert.Equal(3f, curve.Keyframes[0].Time);

        stack.Undo();

        Assert.Equal(0f, curve.Keyframes[0].Time);
    }
}
