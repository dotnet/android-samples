using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Webkit;
using Java.Interop;

namespace WebViewJavaScriptInterface
{
	[Activity (Label = "Mono WebView ScriptInterface", MainLauncher = true)]
	public class JavaScriptInterfaceActivity : Activity
	{
		const string html = @"
	<html>
	<body>
	<p>This is a paragraph.</p>
	<button type=""button"" onClick=""Foo.bar('test message')"">Click Me!</button>
	</body>
	</html>";
	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
	
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
	
			WebView view = FindViewById<WebView> (Resource.Id.web);
			view.Settings.JavaScriptEnabled = true;
			view.SetWebChromeClient (new WebChromeClient ());
			view.AddJavascriptInterface (new Foo (this), "Foo");
			view.LoadData (html, "text/html", null);
		}
	}
	
	class Foo : Java.Lang.Object
	{
		public Foo (Context context)
		{
			this.context = context;
		}
		
		public Foo (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}
	
		Context context;
	
		[Export ("bar")]
		// to become consistent with Java/JS interop convention, the argument cannot be System.String.
		public void Bar (Java.Lang.String message)
		{
			Console.WriteLine ("Foo.Bar invoked!");
			Toast.MakeText (context, "This is a Toast from C#! " + message, ToastLength.Short).Show ();
		}
	}
	
}
