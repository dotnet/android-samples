namespace com.xamarin.evolve2013.animationsdemo
{
    using System;

    using Android.Animation;
    using Android.Content;
    using Android.Graphics;
    using Android.Util;
    using Android.Views;

    public class KarmaMeter : View
    {
        private const int DefaultHeight = 20;
        private const int DefaultWidth = 120;

        private Paint _negativePaint;
        private double _position = 0.5;
        private Paint _positivePaint;

        public KarmaMeter(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
            Initialize();
        }

        public KarmaMeter(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public double KarmaValue
        {
            get { return _position; }
            set
            {
                _position = Math.Max(0f, Math.Min(value, 1f));
                Invalidate();
            }
        }

        public void SetKarmaValue(double value, bool animate)
        {
            if (!animate)
            {
                KarmaValue = value;
                return;
            }

            ValueAnimator animator = ValueAnimator.OfFloat((float)_position, (float)Math.Max(0f, Math.Min(value, 1f)));
            animator.SetDuration(500);

            animator.Update += (sender, e) => KarmaValue = (double)e.Animation.AnimatedValue;
            animator.Start();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            float middle = canvas.Width * (float)_position;

            canvas.DrawPaint(_negativePaint);

            canvas.DrawRect(0, 0, middle, canvas.Height, _positivePaint);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            SetMeasuredDimension(width < DefaultWidth ? DefaultWidth : width, DefaultHeight);
        }

        private void Initialize()
        {
            _positivePaint = new Paint
                                 {
                                     AntiAlias = true,
                                     Color = Color.Rgb(0x99, 0xcc, 0),
                                 };
            _positivePaint.SetStyle(Paint.Style.FillAndStroke);

            _negativePaint = new Paint
                                 {
                                     AntiAlias = true,
                                     Color = Color.Rgb(0xff, 0x44, 0x44)
                                 };
            _negativePaint.SetStyle(Paint.Style.FillAndStroke);
        }
    }
}
