using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.OS;

namespace Novell.DroidSamples.MineSweeper
{
	public class MineFieldView : View
	{
		public readonly int NumColumns = 10;
		public readonly int NumRows = 10;
		public readonly int DefaultCellSize = 16; // FIXME

		int cell_size;

		public MineFieldView (Context context, int num_columns, int num_rows)
			: base (context)
		{
			this.NumColumns = num_columns;
			this.NumRows = num_rows;
		}

		public MineFieldView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			Log ("ATTR SET CTOR");
			NumColumns = attrs.GetAttributeIntValue (R.attr.mine_field_columns, 10);
			NumRows = attrs.GetAttributeIntValue (R.attr.mine_field_rows, 10);
		}

		public MineFieldView (IntPtr handle)
			: base (handle)
		{ }

		protected void Log (string format, params object[] args)
		{
			var message = String.Format (format, args);
			Logger.Log (LogLevel.Verbose, AndroidEnvironment.AndroidLogAppName, message);
		}

		protected override void OnFinishInflate ()
		{
			Log ("ON FINISH INFLATE: {0}x{1}", NumColumns, NumRows);

			base.OnFinishInflate ();
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int width_size = View.MeasureSpec.GetSize (widthMeasureSpec);
			int width_mode = View.MeasureSpec.GetMode (widthMeasureSpec);

			int height_size = View.MeasureSpec.GetSize (heightMeasureSpec);
			int height_mode = View.MeasureSpec.GetMode (heightMeasureSpec);

			Log ("MEASURE: {0}/{1:x} - {2}/{3:x}", width_size, width_mode, height_size, height_mode);

			if ((width_mode == MeasureSpec.AtMost) || (width_mode == MeasureSpec.Exactly) ||
			    (height_mode == MeasureSpec.AtMost) || (height_mode == MeasureSpec.Exactly))
				cell_size = Math.Min (width_size / NumColumns, height_size / NumRows);
			else
				cell_size = DefaultCellSize;

			Log ("MEASURE DONE: {0}", cell_size);

			int total_width = NumColumns * cell_size;
			int total_height = NumRows * cell_size;

			SetMeasuredDimension (total_width, total_height);
		}

		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			Log ("ON LAYOUT: {0} - {1} {2} {3} {4}", changed, left, top, right, bottom);

			base.OnLayout (changed, left, top, right, bottom);
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			Log ("ON DRAW: {0} {1} {2}", canvas.Width, canvas.Height, cell_size);

			// canvas.DrawColor (Color.Cyan);

			var line_paint = new Paint ();
			line_paint.Color = Color.Cyan;
			line_paint.StrokeWidth = 10;
			line_paint.SetStyle (Paint.Style.Stroke);

			canvas.DrawLine (0.0f, 0.0f, 100.0f, 100.0f, line_paint);
			return;

			var bmp = GetResourceBitmap (R.drawable.bomb_32x32);
			canvas.DrawBitmap (bmp, 70, 70, line_paint);

			for (int i = 0; i < NumColumns; i++) {
				canvas.DrawLine (i * cell_size, 0, i * cell_size, canvas.Height, line_paint);
			}
			for (int i = 0; i < NumRows; i++) {
				canvas.DrawLine (0, i * cell_size, canvas.Width, i * cell_size + 5, line_paint);
			}

			canvas.DrawRect (10, 10, 50, 50, line_paint);
		}

		private Bitmap GetResourceBitmap (int bmp_res_id)
		{
			Drawable d = Resources.GetDrawable (bmp_res_id);
			Log ("TEST: {0}", d != null);

			int w = d.IntrinsicWidth;
			int h = d.IntrinsicHeight;

			Log ("TEST #0: {0} {1}", w, h);

			w = 32;
			h = 32;

			var bmp = Bitmap.CreateBitmap (w, h, Bitmap.Config.Argb8888);
			Log ("TEST #1: {0}", bmp != null);
			Canvas c = new Canvas (bmp);
			d.SetBounds (0, 0, w - 1, h - 1);
			d.Draw (c);

			return bmp;
		}
	}
}
