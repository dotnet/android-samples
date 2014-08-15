
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Animation;
using Android.Graphics;

namespace FlashlightSample
{

	public class MyHandler : Handler
	{
		PartyLightView v;
		public MyHandler (PartyLightView view)
		{
			v = view;
		}

		//Continually sends and recieves messages to cycle through the colors
		public override void HandleMessage (Message msg)
		{
			//Cycle through every combination of the colors in the array
			v.current_color = v.GetColor (v.progress, v.colors [v.from_color_index], v.colors [v.to_color_index]);
			v.PostInvalidate ();
			v.progress += 0.1f;
			if (v.progress > 1.0) {
				v.from_color_index = v.to_color_index;
				v.to_color_index++;
				if (v.to_color_index >= v.colors.Length)
					v.to_color_index = 0;

			}
			v.handler.SendEmptyMessageDelayed (0, 100);
		}
	}

	public class PartyLightView : View
	{

		public int[] colors = new int[] {
			Color.Red,
			Color.Green,
			Color.Blue,
			Color.Cyan,
			Color.Magenta
		};

		public int from_color_index;
		public int to_color_index;

		//value between 0 and 1
		public float progress;

		private ArgbEvaluator evaluator;

		public int current_color;

		public MyHandler handler;

		public PartyLightView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public PartyLightView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public PartyLightView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			evaluator = new ArgbEvaluator ();
			handler = new MyHandler (this);
		}

		protected override void OnDraw (Canvas canvas)
		{
			canvas.DrawColor (new Color(current_color));
			base.OnDraw (canvas);
		}

		public void StartCycling()
		{
			handler.SendEmptyMessage (0);
		}

		public void StopCycling()
		{
			handler.RemoveMessages (0);
		}

		//Combines the start color, end color, and progress value (fraction)
		//to generate the ARGB value for the resulting color when combined
		public int GetColor(float fraction, int colorStart, int colorEnd)
		{
			int startInt = colorStart;
			int startA = (startInt >> 24) & 0xff;
			int startR = (startInt >> 16) & 0xff;
			int startG = (startInt >> 8) & 0xff;
			int startB = startInt & 0xff;

			int endInt = colorEnd;
			int endA = (endInt >> 24) & 0xff;
			int endR = (endInt >> 16) & 0xff;
			int endG = (endInt >> 8) & 0xff;
			int endB = endInt & 0xff;

			return (startA + (int)(fraction * (endA - startA))) << 24 |
				(startR + (int)(fraction * (endR - startR))) << 16 |
				(startG + (int)(fraction * (endG - startG))) << 8 |
				((startB + (int)(fraction * (endB - startB))));
		}
	}
}

