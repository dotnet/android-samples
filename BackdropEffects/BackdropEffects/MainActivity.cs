using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Debug = System.Diagnostics.Debug;

namespace BackdropEffects;

[Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@android:style/Theme.Material.NoActionBar")]
public class MainActivity : Activity
{
    private LinearLayout? _demoCard;
    private TextView? _status;
    private RenderEffect? _activeEffect;
    private float _blurRadius = 24;
    private EffectMode _mode = EffectMode.Backdrop;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var root = new FrameLayout(this);

        var backdrop = new AnimatedBackdropView(this);
        root.AddView(backdrop, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent));

        _demoCard = CreateDemoCard();
        var demoParams = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent,
            GravityFlags.Center);
        demoParams.SetMargins(Dp(28), 0, Dp(28), Dp(120));
        root.AddView(_demoCard, demoParams);

        var controls = CreateControls(backdrop);
        var controlsParams = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent,
            GravityFlags.Bottom);
        controlsParams.SetMargins(Dp(16), Dp(16), Dp(16), Dp(20));
        root.AddView(controls, controlsParams);

        SetContentView(root);
        ApplyEffect();
    }

    protected override void OnDestroy()
    {
        _activeEffect?.Dispose();
        base.OnDestroy();
    }

    private LinearLayout CreateDemoCard()
    {
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        card.SetGravity(GravityFlags.CenterHorizontal);
        card.SetPadding(Dp(24), Dp(28), Dp(24), Dp(28));
        card.Background = RoundedBackground(Color.Argb(150, 245, 248, 255), 28);

        var eyebrow = new TextView(this)
        {
            Text = "ANDROID 17 QPR2",
            TextSize = 12,
            LetterSpacing = 0.16f,
            Gravity = GravityFlags.Center,
        };
        eyebrow.SetTextColor(Color.Rgb(36, 62, 105));

        var title = new TextView(this)
        {
            Text = "Backdrop glass",
            TextSize = 30,
            Gravity = GravityFlags.Center
        };
        title.SetTextColor(Color.Rgb(12, 26, 52));
        title.SetPadding(0, Dp(8), 0, Dp(8));

        var description = new TextView(this)
        {
            Text = "Backdrop blur keeps this content crisp while filtering the animated scene behind the card.",
            TextSize = 16,
            Gravity = GravityFlags.Center
        };
        description.SetTextColor(Color.Rgb(33, 48, 73));

        card.AddView(eyebrow);
        card.AddView(title);
        card.AddView(description);
        return card;
    }

    private LinearLayout CreateControls(AnimatedBackdropView backdrop)
    {
        var controls = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        controls.SetPadding(Dp(18), Dp(14), Dp(18), Dp(14));
        controls.Background = RoundedBackground(Color.Argb(235, 13, 20, 38), 24);

        var modeRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        modeRow.SetGravity(GravityFlags.Center);

        modeRow.AddView(CreateModeButton("Backdrop", EffectMode.Backdrop), WeightedButtonParams());
        modeRow.AddView(CreateModeButton("Foreground", EffectMode.Foreground), WeightedButtonParams());
        modeRow.AddView(CreateModeButton("None", EffectMode.None), WeightedButtonParams());

        var radiusLabel = new TextView(this)
        {
            Text = "Blur radius: 24 px",
            TextSize = 14
        };
        radiusLabel.SetTextColor(Color.White);
        radiusLabel.SetPadding(Dp(4), Dp(10), 0, 0);

        var radius = new SeekBar(this)
        {
            Max = 64,
            Progress = 24
        };
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
            radius.Min = 1;

        radius.ProgressChanged += (_, e) =>
        {
            if (e.Progress == 0)
            {
                radius.Progress = 1;
                return;
            }

            _blurRadius = e.Progress;
            radiusLabel.Text = $"Blur radius: {_blurRadius} px";
            ApplyEffect();
        };

        var footerRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        footerRow.SetGravity(GravityFlags.CenterVertical);
        _status = new TextView(this)
        {
            TextSize = 13
        };
        _status.SetTextColor(Color.Rgb(190, 205, 230));

        var pauseButton = new Button(this)
        {
            Text = "Pause"
        };
        pauseButton.Click += (_, _) =>
        {
            backdrop.IsPaused = !backdrop.IsPaused;
            pauseButton.Text = backdrop.IsPaused ? "Resume" : "Pause";
        };

        footerRow.AddView(_status, new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1));
        footerRow.AddView(pauseButton);

        controls.AddView(modeRow);
        controls.AddView(radiusLabel);
        controls.AddView(radius);
        controls.AddView(footerRow);
        return controls;
    }

    private Button CreateModeButton(string text, EffectMode mode)
    {
        var button = new Button(this)
        {
            Text = text,
            TextSize = 12
        };
        button.Click += (_, _) =>
        {
            _mode = mode;
            ApplyEffect();
        };
        return button;
    }

    private void ApplyEffect()
    {
        if (_demoCard is null || _status is null)
            return;

        if (OperatingSystem.IsAndroidVersionAtLeast(31))
            _demoCard.SetRenderEffect(null);

        if (OperatingSystem.IsAndroidVersionAtLeast(37, 2))
            _demoCard.SetBackdropRenderEffect(null);

        _activeEffect?.Dispose();
        _activeEffect = null;

        if (_mode == EffectMode.None)
        {
            _status.Text = "No render effect";
            return;
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            _status.Text = "Render effects require API 31 or later";
            return;
        }

        if (_mode == EffectMode.Backdrop && !OperatingSystem.IsAndroidVersionAtLeast(37, 2))
        {
            _status.Text = "Backdrop blur requires API 37.2";
            return;
        }

        var tileMode = Shader.TileMode.Clamp;
        Debug.Assert(tileMode is not null);
        var effect = RenderEffect.CreateBlurEffect(
            _blurRadius,
            _blurRadius,
            tileMode);
        ArgumentNullException.ThrowIfNull(effect);
        _activeEffect = effect;

        if (_mode == EffectMode.Backdrop)
        {
            _demoCard.SetBackdropRenderEffect(_activeEffect);
            _status.Text = "Filtering pixels behind the card";
            return;
        }

        _demoCard.SetRenderEffect(_activeEffect);
        _status.Text = "Filtering the card and its content";
    }

    private GradientDrawable RoundedBackground(Color color, int radiusDp)
    {
        var background = new GradientDrawable();
        background.SetColor(color);
        background.SetCornerRadius(Dp(radiusDp));
        return background;
    }

    private static LinearLayout.LayoutParams WeightedButtonParams() =>
        new(0, ViewGroup.LayoutParams.WrapContent, 1);

    private int Dp(int value)
    {
        var resources = Resources;
        Debug.Assert(resources is not null);
        var displayMetrics = resources.DisplayMetrics;
        Debug.Assert(displayMetrics is not null);
        return (int)Math.Round(value * displayMetrics.Density);
    }

    private enum EffectMode
    {
        Backdrop,
        Foreground,
        None
    }
}