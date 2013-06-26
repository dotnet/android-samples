/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
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
using Android.Support.V4.Widget;
using Android.Support.V4.View;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * Sliding pane layout that only allows
 * sliding if the slide gesture originates from a gutter
 * 
 */
	public class CustomSlidingPaneLayout : SlidingPaneLayout {

		private static int MEDIUM_DENSITY_SCREEN_DPI = 160;
		public static int DEFAULT_GUTTER_SIZE_DP = 32;

		float gutter;
		bool ignoreEvents = false;

		public CustomSlidingPaneLayout(Context context)
			: this(context, null) {
		}

		public CustomSlidingPaneLayout(Context context, IAttributeSet attrs)
			: this(context, attrs, 0) {
		}

		public CustomSlidingPaneLayout(Context context, IAttributeSet attrs,
		                               int defStyle) 
			: base (context, attrs, defStyle)
		{

			DisplayMetrics metrics = context.Resources.DisplayMetrics;
			gutter = DEFAULT_GUTTER_SIZE_DP * ((int) metrics.DensityDpi / (float) MEDIUM_DENSITY_SCREEN_DPI);
		}

		override
			public bool OnInterceptTouchEvent(MotionEvent ev) {
			int action = MotionEventCompat.GetActionMasked(ev);

			switch((MotionEventActions) action) {
			case MotionEventActions.Down:
				if (ev.GetX () > gutter && !IsOpen) {
					ignoreEvents = true;
					return false;
				}
				break;

			case MotionEventActions.Cancel:
			case MotionEventActions.Up:
				if (ignoreEvents) {
					ignoreEvents = false;
					return false;
				}
				break;
			}

			return !ignoreEvents && base.OnInterceptTouchEvent(ev);
		}
	}
}
