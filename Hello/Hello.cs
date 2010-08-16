#define CATCH
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Runtime;
using Android.Views;
using Javax.Net;
using Javax.Microedition.Khronos.Egl;

namespace Mono.Samples.Hello
{
	public class HelloActivity : Activity, View.IOnTouchListener
	{
		TextView textview;

		public HelloActivity (IntPtr handle) : base (handle)
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
#if MONODROID_TIMING
			Logger.Log(LogLevel.Info, "MonoDroid-Timing", "HelloActivity.OnCreate: time: " + (DateTime.Now - new DateTime (1970, 1, 1)).TotalMilliseconds);
#endif
			base.OnCreate (bundle);

			textview = new TextView (this);
			textview.Text = "Hello MonoDroid.  Mono loves you.\n\nEmbedded\u0000Nulls";

			// Sanity checking to ensure that our binding stays correct.
			var v = ServerSocketFactory.Default;
			Logger.Log(LogLevel.Info, "HelloApp", "ServerSocketFactory.Default=" + v);
			var egl = EGLContext.EGL;
			Logger.Log(LogLevel.Info, "HelloApp", "EGLContext.EGL=" + egl);
			IEGL10 egl10 = egl.JavaCast<IEGL10>();
			Logger.Log(LogLevel.Info, "HelloApp", "(IEGL10) EGLContext.EGL=" + egl10.GetType().FullName);

			var list = new Java.Util.ArrayList<string>();
			Java.Util.IIterator<string> iterator = list.Iterator ();

#if CATCH
			try {
#endif
				Logger.Log (LogLevel.Info, "HelloApp", "calling setDefaultKeyMode...");
				// NOTE: calling base.SetDefaultKeyMode() causes the app to exit
				// unexpectedly when the Home button is pressed.
				// this is NOT a MonoDroid bug; the equivalent Java behaves the same.
				base.SetDefaultKeyMode (-100);
				Logger.Log (LogLevel.Info, "HelloApp", "after setDefaultKeyMode...");
#if CATCH
			}
			catch (Exception e) {
				Logger.Log (LogLevel.Info, "HelloApp", "Yay, exception caught!" + e);
				textview.Text += "\n\nEXPECTED EXCEPTION:\n" + e.ToString ();
			}
#endif

			SetContentView (textview);
			textview.SetOnTouchListener (this);
		}

		public bool OnTouch (View v, MotionEvent e)
		{
			Logger.Log(LogLevel.Info, "HelloApp", "OnTouchListener.OnTouch: v=" + v + "; e=" + e);
			return false;
		}
	}
}

