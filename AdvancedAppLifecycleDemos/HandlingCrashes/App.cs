using System;
using Android.Content;
using System.Threading.Tasks;
using Android.Util;
using System.Threading;

namespace AppLifecycle
{
	/// <summary>
	/// Singleton class for Application wide objects. 
	/// </summary>
	public class App
	{
		// declarations
		public event EventHandler<EventArgs> Initialized = delegate {};
		protected readonly string logTag = "!!!!!!! App";

		public readonly bool RestartMainActivityOnCrash = false;

		// properties
		public bool IsInitialized { get; set; }

		public static App Current
		{
			get { return current; }
		} private static App current;

		static App ()
		{
			current = new App();
		}
		protected App () 
		{
			// subscribe to app wide unhandled exceptions so that we can log them.
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

			// any work here is likely to be blocking (static constructors run on whatever thread that first 
			// access its instance members, which in our case is an activity doing an initialization check),
			// so we want to do it on a background thread
			new Task (() => { 
				
				// add a little wait time, to illustrate a loading event
				// TODO: Replace this with real work in your app, such as starting services,
				// database init, web calls, etc.
				Thread.Sleep (2500);
				
				// set our initialization flag so we know that we're all setup
				this.IsInitialized = true;
				// raise our intialized event
				this.Initialized (this, new EventArgs ());
				Log.Debug (logTag, "App initialized, setting Initialized = true");
			}).Start ();

		}

		/// <summary>
		/// When app-wide unhandled exceptions are hit, this will handle them. Be aware however, that typically
		/// android will be destroying the process, so there's not a lot you can do on the android side of things,
		/// but your xamarin code should still be able to work. so if you have a custom err logging manager or 
		/// something, you can call that here. You _won't_ be able to call Android.Util.Log, because Dalvik
		/// will destroy the java side of the process.
		/// </summary>
		protected void HandleUnhandledException (object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception) args.ExceptionObject;

			// log won't be available, because dalvik is destroying the process
			//Log.Debug (logTag, "MyHandler caught : " + e.Message);
			// instead, your err handling code shoudl be run:
			Console.WriteLine ("========= MyHandler caught : " + e.Message);
		}
	}
}

