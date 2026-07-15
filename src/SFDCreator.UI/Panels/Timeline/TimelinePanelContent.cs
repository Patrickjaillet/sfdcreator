using SFDCreator.Core.Timeline.Sync;
using SFDCreator.UI.Hosting;
using SFDCreator.UI.Theming;
using SkiaSharp;

namespace SFDCreator.UI.Panels.Timeline;

public sealed class TimelinePanelContent : IUiPanelContent
{
    private const float RulerHeight = 24f;
    private const float TrackHeight = 26f;
    private const float LeftMargin = 12f;
    private const float RightMargin = 12f;

    private static readonly SKFont Font = new(SKTypeface.Default, 11f);

    private readonly TimelineModel _model;
    private int? _draggingKeyframeIndex;
    private bool _draggingPlayhead;

    public TimelinePanelContent(TimelineModel model)
    {
        _model = model;
    }

    public Action<int, float>? KeyframeMoved { get; set; }

    public Action<float>? PlayheadScrubbed { get; set; }

    public Func<float, float>? SnapToGrid { get; set; }

    public BpmGrid? Grid { get; set; }

    public void Render(SKCanvas canvas, SKRect bounds, UiInputState input)
    {
        var theme = UiTheme.Current;

        using var background = new SKPaint { Color = theme.Panel, Style = SKPaintStyle.Fill };
        canvas.DrawRect(bounds, background);

        var rulerLeft = bounds.Left + LeftMargin;
        var rulerWidth = Math.Max(bounds.Width - LeftMargin - RightMargin, 1f);
        var trackTop = bounds.Top + RulerHeight;

        HandleInteraction(input, rulerLeft, rulerWidth, trackTop);

        using var rulerPaint = new SKPaint { IsAntialias = true, Color = theme.Border, Style = SKPaintStyle.Fill };
        canvas.DrawRect(new SKRect(bounds.Left, bounds.Top, bounds.Right, bounds.Top + RulerHeight), rulerPaint);

        if (Grid is { } grid && grid.BeatDurationSeconds > 0f)
        {
            using var gridPaint = new SKPaint { IsAntialias = true, Color = theme.Border, StrokeWidth = 1f };
            var beatCount = (int)(_model.DurationSeconds / grid.BeatDurationSeconds) + 1;
            for (var beat = 0; beat <= beatCount; beat++)
            {
                var beatTime = grid.BeatToTime(beat);
                if (beatTime > _model.DurationSeconds)
                {
                    break;
                }

                var x = _model.TimeToPixel(beatTime, rulerLeft, rulerWidth);
                canvas.DrawLine(x, bounds.Top + RulerHeight, x, bounds.Bottom, gridPaint);
            }
        }

        using var tickPaint = new SKPaint { IsAntialias = true, Color = theme.TextMuted, StrokeWidth = 1f };
        var tickCount = Math.Max((int)_model.DurationSeconds, 1);
        for (var i = 0; i <= tickCount; i++)
        {
            var x = _model.TimeToPixel(i, rulerLeft, rulerWidth);
            canvas.DrawLine(x, bounds.Top + RulerHeight - 6f, x, bounds.Top + RulerHeight, tickPaint);
            canvas.DrawText($"{i}s", x + 2f, bounds.Top + 12f, SKTextAlign.Left, Font, tickPaint);
        }

        using var trackBgEven = new SKPaint { IsAntialias = true, Color = theme.Background, Style = SKPaintStyle.Fill };
        for (var t = 0; t < _model.Tracks.Count; t++)
        {
            var rowRect = new SKRect(bounds.Left, trackTop + (t * TrackHeight), bounds.Right, trackTop + ((t + 1) * TrackHeight));
            if (t % 2 == 0)
            {
                canvas.DrawRect(rowRect, trackBgEven);
            }

            using var labelPaint = new SKPaint { IsAntialias = true, Color = theme.TextMuted };
            canvas.DrawText(_model.Tracks[t], bounds.Left + 4f, rowRect.MidY + 4f, SKTextAlign.Left, Font, labelPaint);
        }

        using var keyframePaint = new SKPaint { IsAntialias = true, Color = theme.Accent, Style = SKPaintStyle.Fill };
        foreach (var keyframe in _model.Keyframes)
        {
            var kx = _model.TimeToPixel(keyframe.Time, rulerLeft, rulerWidth);
            var ky = trackTop + (keyframe.TrackIndex * TrackHeight) + (TrackHeight / 2f);
            DrawDiamond(canvas, kx, ky, 5f, keyframePaint);
        }

        using var playheadPaint = new SKPaint { IsAntialias = true, Color = theme.Text, StrokeWidth = 2f };
        var playheadX = _model.TimeToPixel(_model.PlayheadSeconds, rulerLeft, rulerWidth);
        canvas.DrawLine(playheadX, bounds.Top, playheadX, bounds.Bottom, playheadPaint);
    }

    private void HandleInteraction(UiInputState input, float rulerLeft, float rulerWidth, float trackTop)
    {
        if (input.WasLeftPressedThisFrame)
        {
            var hit = _model.HitTestKeyframe(input.MouseX, input.MouseY, rulerLeft, rulerWidth, trackTop, TrackHeight);
            if (hit is not null)
            {
                _draggingKeyframeIndex = hit;
            }
            else
            {
                _draggingPlayhead = true;
                var time = _model.PixelToTime(input.MouseX, rulerLeft, rulerWidth);
                _model.PlayheadSeconds = time;
                PlayheadScrubbed?.Invoke(time);
            }
        }
        else if (input.IsLeftDown)
        {
            if (_draggingKeyframeIndex is { } index)
            {
                var time = _model.PixelToTime(input.MouseX, rulerLeft, rulerWidth);
                if (SnapToGrid is not null)
                {
                    time = SnapToGrid(time);
                }

                _model.MoveKeyframe(index, time);
                KeyframeMoved?.Invoke(index, time);
            }
            else if (_draggingPlayhead)
            {
                var time = _model.PixelToTime(input.MouseX, rulerLeft, rulerWidth);
                _model.PlayheadSeconds = time;
                PlayheadScrubbed?.Invoke(time);
            }
        }

        if (input.WasLeftReleasedThisFrame)
        {
            _draggingKeyframeIndex = null;
            _draggingPlayhead = false;
        }
    }

    private static void DrawDiamond(SKCanvas canvas, float cx, float cy, float radius, SKPaint paint)
    {
        using var path = new SKPath();
        path.MoveTo(cx, cy - radius);
        path.LineTo(cx + radius, cy);
        path.LineTo(cx, cy + radius);
        path.LineTo(cx - radius, cy);
        path.Close();
        canvas.DrawPath(path, paint);
    }
}
