using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;
using MonoIO.Utilities;
using Android.Text.Format;
using System.Runtime.CompilerServices;
using Math = Java.Lang.Math;

namespace MonoIO
{
	/**
	 * Custom view that draws a vertical time "ruler" representing the chronological
	 * progression of a single day. Usually shown along with {@link BlockView}
	 * instances to give a spatial sense of time.
	 */
	public class TimeRulerView : View
	{
		private int mHeaderWidth = 70;
		private int mHourHeight = 90;
		private bool mHorizontalDivider = true;
		private int mLabelTextSize = 20;
		private int mLabelPaddingLeft = 8;
		private Color mLabelColor = Color.Black;
		private Color mDividerColor = Color.LightGray;
		private int mStartHour = 0;
		private int mEndHour = 23;
		
		public TimeRulerView (Context context) : this(context, null)
		{
		}
	
		public TimeRulerView (Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
		}
	
		public TimeRulerView (Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
	        
			var a = context.ObtainStyledAttributes (attrs, Resource.Styleable.TimeRulerView, defStyle, 0);
	
			mHeaderWidth = a.GetDimensionPixelSize (Resource.Styleable.TimeRulerView_headerWidth, mHeaderWidth);
			mHourHeight = a.GetDimensionPixelSize (Resource.Styleable.TimeRulerView_hourHeight, mHourHeight);
			mHorizontalDivider = a.GetBoolean (Resource.Styleable.TimeRulerView_horizontalDivider, mHorizontalDivider);
			mLabelTextSize = a.GetDimensionPixelSize (Resource.Styleable.TimeRulerView_labelTextSize, mLabelTextSize);
			mLabelPaddingLeft = a.GetDimensionPixelSize (Resource.Styleable.TimeRulerView_labelPaddingLeft, mLabelPaddingLeft);
			mLabelColor = a.GetColor (Resource.Styleable.TimeRulerView_labelColor, mLabelColor);
			mDividerColor = a.GetColor (Resource.Styleable.TimeRulerView_dividerColor, mDividerColor);
			mStartHour = a.GetInt (Resource.Styleable.TimeRulerView_startHour, mStartHour);
			mEndHour = a.GetInt (Resource.Styleable.TimeRulerView_endHour, mEndHour);
	
			a.Recycle ();
		}
		
		/**
	     * Return the vertical offset (in pixels) for a requested time (in
	     * milliseconds since epoch).
	     */
		public int GetTimeVerticalOffset (long timeMillis)
		{
			Time time = new Time (UIUtils.ConferenceTimeZone.ID);
			time.Set (timeMillis);
	
			int minutes = ((time.Hour - mStartHour) * 60) + time.Minute;
			return (minutes * mHourHeight) / 60;
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int hours = mEndHour - mStartHour;
	
			int width = mHeaderWidth;
			int height = mHourHeight * hours;
	
			SetMeasuredDimension (ResolveSize (width, widthMeasureSpec), ResolveSize (height, heightMeasureSpec));
		}
		
		private Paint mDividerPaint = new Paint ();
		private Paint mLabelPaint = new Paint ();
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);
			
			int hourHeight = mHourHeight;
	
			Paint dividerPaint = mDividerPaint;
			dividerPaint.Color = mDividerColor;
			dividerPaint.SetStyle (Paint.Style.Fill);
	
			Paint labelPaint = mLabelPaint;
			labelPaint.Color = mLabelColor;
			labelPaint.TextSize = mLabelTextSize;
			labelPaint.SetTypeface (Typeface.DefaultBold);
			labelPaint.AntiAlias = true;
	
			var metrics = labelPaint.GetFontMetricsInt ();
			int labelHeight = Math.Abs (metrics.Ascent);
			int labelOffset = labelHeight + mLabelPaddingLeft;
	
			int right = Right;
	
			// Walk left side of canvas drawing timestamps
			int hours = mEndHour - mStartHour;
			for (int i = 0; i < hours; i++) {
				int dividerY = hourHeight * i;
				int nextDividerY = hourHeight * (i + 1);
				canvas.DrawLine (0, dividerY, right, dividerY, dividerPaint);
	
				// draw text title for timestamp
				canvas.DrawRect (0, dividerY, mHeaderWidth, nextDividerY, dividerPaint);
	
				// TODO: localize these labels better, including handling
				// 24-hour mode when set in framework.
				int hour = mStartHour + i;
				String label;
				if (hour == 0) {
					label = "12am";
				} else if (hour <= 11) {
					label = hour + "am";
				} else if (hour == 12) {
					label = "12pm";
				} else {
					label = (hour - 12) + "pm";
				}
	
				float labelWidth = labelPaint.MeasureText (label);
	
				canvas.DrawText (label, 0, label.Length, mHeaderWidth - labelWidth - mLabelPaddingLeft, dividerY + labelOffset, labelPaint);
			}
		}
		
		public int GetHeaderWidth ()
		{
			return mHeaderWidth;
		}
	}
}

