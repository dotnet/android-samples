namespace com.xamarin.evolve2013.animationsdemo
{
    using System.Drawing;

    using Android.Content;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Util;
    using Android.Widget;

    using Color = Android.Graphics.Color;

    public class TouchHighlightImageButton : ImageButton
    {
        private Rectangle _cachedBounds;
        private Drawable _foregroundDrawable;

        public TouchHighlightImageButton(Context context)
            : this(context, null)
        {
        }

        public TouchHighlightImageButton(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public TouchHighlightImageButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init();
        }

        protected override void DrawableStateChanged()
        {
            base.DrawableStateChanged();
            // Update the state of the highlight drawable to match
            // the state of the button.
            if (_foregroundDrawable.IsStateful)
            {
                _foregroundDrawable.SetState(GetDrawableState());
            }

            // Trigger a redraw.
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            // Then draw the highlight on top of it. If the button is neither focused
            // nor pressed, the drawable will be transparent, so just the image
            // will be drawn.
            _foregroundDrawable.SetBounds(_cachedBounds.Left, _cachedBounds.Top, _cachedBounds.Right, _cachedBounds.Bottom);
            _foregroundDrawable.Draw(canvas);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            _cachedBounds = new Rectangle(0, 0, w, h);
        }

        private void Init()
        {
            SetBackgroundColor(Color.White);
            SetPadding(0, 0, 0, 0);

            // Retrieve the drawable resource assigned to the Android.Resource.Attribute.SelectableItemBackground
            // theme attribute from the current theme.
            TypedArray a = Context.ObtainStyledAttributes(new[] { Android.Resource.Attribute.SelectableItemBackground });
            _foregroundDrawable = a.GetDrawable(0);
            _foregroundDrawable.SetCallback(this);
            a.Recycle();
        }
    }
}
