using System.Diagnostics;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace BackdropEffects;

internal sealed class AnimatedBackdropView : View
{
    private readonly Paint _backgroundPaint = new();
    private readonly Paint _orbPaint = new(PaintFlags.AntiAlias);
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private LinearGradient? _backgroundGradient;
    private bool _isPaused;

    public AnimatedBackdropView(Context context) : base(context)
    {
    }

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            if (!value)
            {
                _clock.Start();
                PostInvalidateOnAnimation();
            }
            else
            {
                _clock.Stop();
            }
        }
    }

    protected override void OnSizeChanged(int width, int height, int oldWidth, int oldHeight)
    {
        base.OnSizeChanged(width, height, oldWidth, oldHeight);

        _backgroundGradient?.Dispose();
        var tileMode = Shader.TileMode.Clamp;
        Debug.Assert(tileMode is not null);
        _backgroundGradient = new LinearGradient(
            0,
            0,
            width,
            height,
            [Color.Rgb(14, 22, 48), Color.Rgb(33, 25, 72), Color.Rgb(8, 52, 68)],
            null,
            tileMode);
        _backgroundPaint.SetShader(_backgroundGradient);
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        canvas.DrawRect(0, 0, Width, Height, _backgroundPaint);

        var time = (float)_clock.Elapsed.TotalSeconds;
        DrawOrb(
            canvas,
            Width * (0.24f + 0.12f * MathF.Sin(time * 0.7f)),
            Height * (0.30f + 0.10f * MathF.Cos(time * 0.5f)),
            Math.Min(Width, Height) * 0.26f,
            Color.Argb(220, 50, 205, 255));
        DrawOrb(
            canvas,
            Width * (0.74f + 0.12f * MathF.Cos(time * 0.6f)),
            Height * (0.53f + 0.14f * MathF.Sin(time * 0.8f)),
            Math.Min(Width, Height) * 0.31f,
            Color.Argb(205, 255, 83, 164));
        DrawOrb(
            canvas,
            Width * (0.44f + 0.18f * MathF.Sin(time * 0.4f)),
            Height * (0.78f + 0.08f * MathF.Cos(time * 0.9f)),
            Math.Min(Width, Height) * 0.22f,
            Color.Argb(210, 255, 184, 56));

        if (!IsPaused)
            PostInvalidateOnAnimation();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clock.Stop();
            _backgroundGradient?.Dispose();
            _backgroundPaint.Dispose();
            _orbPaint.Dispose();
        }

        base.Dispose(disposing);
    }

    private void DrawOrb(Canvas canvas, float x, float y, float radius, Color color)
    {
        _orbPaint.Color = color;
        canvas.DrawCircle(x, y, radius, _orbPaint);
    }
}
