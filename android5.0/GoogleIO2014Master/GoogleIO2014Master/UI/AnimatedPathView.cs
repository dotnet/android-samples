using System;
using System.Threading;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace GoogleIO2014Master.UI
{
	public class AnimatedPathView : View
	{
		private const string LOG_TAG = "AnimatedPathView";
		private readonly Paint mStrokePaint = new Paint(PaintFlags.AntiAlias);
		private readonly Paint mFillPaint = new Paint(PaintFlags.AntiAlias);

		private readonly SvgHelper svg;
		private int svgResource;

		private readonly object svgLock = new object ();
		private List<SvgHelper.SvgPath> paths = new List<SvgHelper.SvgPath>();
		private Thread loader;
		private readonly Path renderPath = new Path();

		private float phase;
		private float fillAlpha;
		private float fadeFactor;
		private int duration;
		private int fillDuration;
		private int fillOffset;

		//private final 
		public AnimatedPathView (Context context, IAttributeSet attrs)
			:this(context, attrs, 0)
		{

		}
		public AnimatedPathView(Context context, IAttributeSet attrs, int defStyle)
			:base(context, attrs, defStyle)
		{

			mStrokePaint.SetStyle (Paint.Style.Stroke);
			mFillPaint.SetStyle (Paint.Style.Fill);

			TypedArray a = context.ObtainStyledAttributes (attrs, Resource.Styleable.AnimatedPathView, defStyle, 0);
			try {
				if(a != null) {
					mStrokePaint.StrokeWidth = a.GetDimensionPixelSize(Resource.Styleable.AnimatedPathView_strokeWidth,1);
					mStrokePaint.Color = a.GetColor(Resource.Styleable.AnimatedPathView_strokeColor, unchecked((int)0xff000000));
					svg = new SvgHelper (mStrokePaint);
					mFillPaint.Color = a.GetColor(Resource.Styleable.AnimatedPathView_fillColor, unchecked((int)0xff000000));
					phase = a.GetFloat(Resource.Styleable.AnimatedPathView_phase,0.0f);
					duration = a.GetInt(Resource.Styleable.AnimatedPathView_duration,4000);
					fillDuration = a.GetInt(Resource.Styleable.AnimatedPathView_fillDuration,4000);
					fillOffset = a.GetInt(Resource.Styleable.AnimatedPathView_fillOffset,2000);
					fadeFactor = a.GetFloat(Resource.Styleable.AnimatedPathView_fadeFactor,10.0f);
					svgResource = a.GetResourceId(Resource.Styleable.AnimatedPathView_svgPath,0);
				}
			} finally {
				if (a != null)
					a.Recycle ();
			}

		}

		public int FillColor
		{
			get{ return mFillPaint.Color; }
			set{ mFillPaint.Color = new Color(value); }
		}

		public int StrokeColor
		{
			get{ return mStrokePaint.Color; }
			set{ mStrokePaint.Color = new Color(value); }
		}

		public float Phase
		{
			get{ return phase; }
			set{
				phase = value;
				lock (svgLock) {
					UpdatePathsPhaseLocked ();
				}
				Invalidate ();
			}
		}

		public float FillAlpha
		{
			get{ return fillAlpha; }
			set{
				fillAlpha = value;
				Invalidate ();
			}
		}

		public int SvgResource
		{
			get{ return svgResource; }
			set{ svgResource = value; }
		}

		public void Reveal()
		{
			var svgAnimator = ObjectAnimator.OfFloat (this, "phase", 0.0f, 1.0f);
			svgAnimator.SetDuration(duration);
			svgAnimator.Start ();

			FillAlpha = 0.0f;

			var fillAnimator = ObjectAnimator.OfFloat (this, "fillAlpha", 0.0f, 1.0f);
			fillAnimator.SetDuration(fillDuration);
			fillAnimator.StartDelay = fillOffset;
			fillAnimator.Start ();
		}

		public void UpdatePathsPhaseLocked()
		{
			foreach (SvgHelper.SvgPath path in paths) {
				path.renderPath.Reset ();
				path.measure.GetSegment (0.0f, phase * path.length, path.renderPath, true);
			}
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);

			if (loader != null) {
				try {
					loader.Join();
				} catch(Java.Lang.InterruptedException e) {
					Log.Error(LOG_TAG,"Unexpected error", e);
				}
			}

			loader = new Thread(new ThreadStart(delegate {
				svg.Load(Context,svgResource);
				lock (svgLock) {
					paths = svg.GetPathsForViewport(
						w - PaddingLeft - PaddingRight,
						h - PaddingTop - PaddingBottom);
					UpdatePathsPhaseLocked();
				}
			}));
			loader.Name = "SVG Loader";
			loader.Start();
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			lock (svgLock) {
				canvas.Save ();
				canvas.Translate (PaddingLeft, PaddingTop);
				mFillPaint.Alpha = (int)(fillAlpha * 255.0f);
				foreach (SvgHelper.SvgPath path in paths) {
					var alpha = (int)(Math.Min (phase * fadeFactor, 1.0f) * 255.0f);
					path.paint.Alpha = alpha;

					canvas.DrawPath (path.path, mFillPaint);
					canvas.DrawPath (path.renderPath, path.paint);
				}
				canvas.Restore ();
			}
		}
	}
}

