/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using System.Collections.Generic;
using Android.Graphics;
using Xamarin.ActionbarSherlockBinding.App;
using ActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;
using Xamarin.UrbanAirship.Utils;
using ViewPager = Android.Support.V4.View.ViewPager;
using Android.Util;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
		/**
	 * A ViewPager that allows touch events to be
	 * enabled and disabled
	 * 
	 */
		public class CustomViewPager : ViewPager {

			private bool isTouchEnabled = true;

			public CustomViewPager(Context context)
				: base(context){
			}

			public CustomViewPager(Context context, IAttributeSet attrs)
				: base (context, attrs) {
			}

			override
				public bool OnTouchEvent(MotionEvent evt) {
				return isTouchEnabled && base.OnTouchEvent(evt);
			}

			override
				public bool OnInterceptTouchEvent(MotionEvent evt) {
				return isTouchEnabled && base.OnInterceptTouchEvent(evt);
			}

			/**
	     * Sets touch to be disabled or enabled
	     * @param isTouchEnabled <code>true</code> to enable touch, <code>false</code> to disable
	     */
			public void EnableTouchEvents(bool isTouchEnabled) {
				this.isTouchEnabled = isTouchEnabled;
			}
		}
}
