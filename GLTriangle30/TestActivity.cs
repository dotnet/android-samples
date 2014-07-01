
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
using Android.Content.PM;

namespace Mono.Samples.GLTriangle30
{
	[Activity (Label = "Compatibility Check", MainLauncher = true)]
	public class TestActivity : Activity
	{
		private static string tag = "GLVersion";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Test);
			if (SupportedOpenGLVersion () >= 3) {
				Log.Info (tag, "Supported hardware. Launching the demo.");
				var launch = new Intent (this, typeof(GLTriangle30Activity));
				StartActivity (launch);
				Finish ();
			} else {
				var textout = FindViewById<TextView> (Resource.Id.textView1);
				textout.Text = GetString (Resource.String.unsupported);
				Log.Info (tag, "Unsupported hardware. Displaying notification screen");
			}
		}

		/// <summary>
		/// Gets the supported OpenGL ES version of device.
		/// </summary>
		/// <returns>Hieghest supported version of OpenGL ES</returns>
		private long SupportedOpenGLVersion ()
		{
			//based on https://android.googlesource.com/platform/cts/+/master/tests/tests/graphics/src/android/opengl/cts/OpenGlEsVersionTest.java
			var featureInfos = PackageManager.GetSystemAvailableFeatures ();
			if (featureInfos != null && featureInfos.Length > 0) {
				foreach (FeatureInfo info in featureInfos) {
					// Null feature name means this feature is the open gl es version feature.
					if (info.Name == null) {
						if (info.ReqGlEsVersion != FeatureInfo.GlEsVersionUndefined)
							return GetMajorVersion (info.ReqGlEsVersion);
						else
							return 0L;
					}
				}
			}
			return 0L;
		}

		private static long GetMajorVersion (long raw)
		{
			//based on https://android.googlesource.com/platform/cts/+/master/tests/tests/graphics/src/android/opengl/cts/OpenGlEsVersionTest.java
			long cleaned = ((raw & 0xffff0000) >> 16);
			Log.Info (tag, "OpenGL ES major version: " + cleaned);
			return cleaned;
		}
	}
}

