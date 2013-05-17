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
	}
}

