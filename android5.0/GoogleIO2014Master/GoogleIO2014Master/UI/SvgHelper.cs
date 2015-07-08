using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content;
using Android.Util;
using Com.Caverock.Androidsvg;

namespace GoogleIO2014Master.UI
{
	public class SvgHelper
	{
		private readonly string LOG_TAG = "SVG";

		private List<SvgPath> paths;
		private Paint sourcepaint;

		private SVG svg;

		public SvgHelper(Paint sourcePaint)
		{
			paths = new List<SvgPath> ();
			this.sourcepaint = sourcepaint;
		}

		public void Load(Context context, int svgResource)
		{
			if (svg != null)
				return;
			try {
				svg = SVG.GetFromResource(context,svgResource);
				svg.DocumentPreserveAspectRatio = PreserveAspectRatio.Unscaled;
			} catch (Exception e) {
				Log.Error (LOG_TAG, "Could not load specified SVG resource", e);
			}
		}

		public class SvgPath {
			private Region region = new Region();
			private Region maxClip = new Region(int.MinValue,int.MinValue,int.MaxValue,int.MaxValue);
			public Path path;
			public Path renderPath = new Path();
			public Paint paint;
			public float length;
			public Rect bounds;
			public PathMeasure measure;

			public SvgPath(Path path, Paint paint)
			{
				this.path = path;
				this.paint = paint;
				measure = new PathMeasure (path, false);
				this.length = measure.Length;
				region.SetPath (path, maxClip);
				bounds = region.Bounds;
			}


		}

		public List<SvgPath> GetPathsForViewport(int width, int height)
		{
			paths.Clear ();
			var canvas = new SvgCanvas (this, width, height);

			RectF viewBox = svg.DocumentViewBox;
			float scale = Math.Min (width / viewBox.Width (), height / viewBox.Height ());
			canvas.Translate(
				(width - viewBox.Width() * scale) / 2.0f,
				(height - viewBox.Height() * scale) / 2.0f);
			canvas.Scale (scale, scale);
			svg.RenderToCanvas (canvas);
			return paths;

		}
		private class SvgCanvas : Canvas
		{
			private Matrix matrix = new Matrix();
			private SvgHelper helper;
			private int width;
			private int height;
			public SvgCanvas(SvgHelper h, int width, int height) : base()
			{
				helper = h;
				this.width = width;
				this.height = height;

			}
			public override int Width {
				get {
					return width;
				}
			}

			public override int Height {
				get {
					return height;
				}
			}

			public override void DrawPath (Path path, Paint paint)
			{
				Path dst = new Path ();

				GetMatrix (matrix);
				path.Transform (matrix, dst);
				helper.paths.Add(new SvgPath(dst,new Paint(helper.sourcepaint)));
			}
		}


	}


}