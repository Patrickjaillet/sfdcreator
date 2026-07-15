using SFDCreator.Core.Timeline.Sequencing;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class MasterTimelineTests
{
    private static MasterTimeline BuildTwoSegmentTimeline(TransitionSpec secondSegmentTransition)
    {
        var timeline = new MasterTimeline();
        timeline.AddSegment(new TimelineSegment("A", 0f, 6f, TransitionSpec.Cut));
        timeline.AddSegment(new TimelineSegment("B", 6f, 6f, secondSegmentTransition));
        return timeline;
    }

    [Fact]
    public void Evaluate_WithinSteadySegment_ReturnsNoBlend()
    {
        var timeline = BuildTwoSegmentTimeline(new TransitionSpec(TransitionKind.Crossfade, 1f));

        var blend = timeline.Evaluate(2f);

        Assert.Equal("A", blend.Current.Name);
        Assert.Null(blend.Next);
        Assert.Equal(0f, blend.BlendWeight);
    }

    [Fact]
    public void Evaluate_WithinCrossfadeWindow_RampsBlendWeight()
    {
        var timeline = BuildTwoSegmentTimeline(new TransitionSpec(TransitionKind.Crossfade, 1f));

        var start = timeline.Evaluate(5f);
        var middle = timeline.Evaluate(5.5f);
        var end = timeline.Evaluate(5.999f);

        Assert.Equal("A", start.Current.Name);
        Assert.Equal("B", start.Next!.Name);
        Assert.Equal(0f, start.BlendWeight, 2);
        Assert.Equal(0.5f, middle.BlendWeight, 1);
        Assert.True(end.BlendWeight > 0.9f);
    }

    [Fact]
    public void Evaluate_WithCutTransition_NeverBlends()
    {
        var timeline = BuildTwoSegmentTimeline(TransitionSpec.Cut);

        var justBeforeCut = timeline.Evaluate(5.999f);
        var justAfterCut = timeline.Evaluate(6.001f);

        Assert.Null(justBeforeCut.Next);
        Assert.Equal("A", justBeforeCut.Current.Name);
        Assert.Null(justAfterCut.Next);
        Assert.Equal("B", justAfterCut.Current.Name);
    }

    [Fact]
    public void Evaluate_WrapsTimeAroundTotalDuration()
    {
        var timeline = BuildTwoSegmentTimeline(TransitionSpec.Cut);

        var blend = timeline.Evaluate(13f);

        Assert.Equal("A", blend.Current.Name);
    }

    [Fact]
    public void Evaluate_NegativeTime_WrapsIntoValidRange()
    {
        var timeline = BuildTwoSegmentTimeline(TransitionSpec.Cut);

        var blend = timeline.Evaluate(-1f);

        Assert.Equal("B", blend.Current.Name);
    }

    [Fact]
    public void TotalDuration_IsMaxOfSegmentEnds()
    {
        var timeline = BuildTwoSegmentTimeline(TransitionSpec.Cut);

        Assert.Equal(12f, timeline.TotalDuration);
    }

    [Fact]
    public void Evaluate_WithNoSegments_Throws()
    {
        var timeline = new MasterTimeline();

        Assert.Throws<InvalidOperationException>(() => timeline.Evaluate(0f));
    }
}
