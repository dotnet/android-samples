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

namespace RotationDemo
{
	[Activity (Label = "CodeLayoutActivity", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation)]			
	public class CodeLayoutActivity : Activity
	{
		TextView _tv;
		string _timestamp;
		RelativeLayout.LayoutParams _layoutParamsPortrait;
		RelativeLayout.LayoutParams _layoutParamsLandscape;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			_timestamp = DateTime.Now.ToLongTimeString ();
			
			// create a layout
			var rl = new RelativeLayout (this);
			var layoutParams = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			rl.LayoutParameters = layoutParams;
			
			// get the initial orientation
			var surfaceOrientation = this.WindowManager.DefaultDisplay.Rotation;
			
			// create the portrait and landscape layout
			_layoutParamsPortrait = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);		
			_layoutParamsLandscape = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
			_layoutParamsLandscape.LeftMargin = 100;
			_layoutParamsLandscape.TopMargin = 100;
			
			// create the TextView an assign the initial layout params
			_tv = new TextView (this);
			
			if (surfaceOrientation == SurfaceOrientation.Rotation0 || surfaceOrientation == SurfaceOrientation.Rotation180) {
				_tv.LayoutParameters = _layoutParamsPortrait;
			} else {
				_tv.LayoutParameters = _layoutParamsLandscape;
			}
			
			_tv.Text = "Programmatic layout. Timestamp = " + _timestamp;
			
			rl.AddView (_tv);
	
			SetContentView (rl);
		}
		
		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			
			Console.WriteLine ("config changed");
			
			if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait) {
				_tv.LayoutParameters = _layoutParamsPortrait;
				_tv.Text = "Changed to portrait. Timestamp = " + _timestamp;
			} else if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape) {
				_tv.LayoutParameters = _layoutParamsLandscape;
				_tv.Text = "Changed to landscape. Timestamp = " + _timestamp;
			}
		}
		
// remove ConfigurationChanges from ActvityAttribute for this code to coincide with article
//		protected override void OnCreate (Bundle bundle)
//		{
//			base.OnCreate (bundle);
//			
//			// create a layout
//			var rl = new RelativeLayout (this);
//			var layoutParams = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
//			rl.LayoutParameters = layoutParams;
//			
//			// get the initial orientation
//			var surfaceOrientation = this.WindowManager.DefaultDisplay.Rotation;
//			Console.WriteLine ("surface orientation = {0}", surfaceOrientation);
//			
//			// create layout based upon orientation
//			RelativeLayout.LayoutParams tvLayoutParams;
//			
//			if (surfaceOrientation == SurfaceOrientation.Rotation0 || surfaceOrientation == SurfaceOrientation.Rotation180) {
//				tvLayoutParams = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
//			} else {
//				tvLayoutParams = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
//				tvLayoutParams.LeftMargin = 100;
//				tvLayoutParams.TopMargin = 100;
//			}
//			
//			// add controls
//			var tv = new TextView (this);
//			tv.LayoutParameters = tvLayoutParams;
//			tv.Text = "Programmatic layout";			
//			rl.AddView (tv);
//	
//			SetContentView (rl);
//		}
	}
}