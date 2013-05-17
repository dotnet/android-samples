using System;
using Android.Content;
using AppLifecycle.Services;
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
		// events
		public event EventHandler<EventArgs> Initialized = delegate {};
		public event EventHandler<ServiceConnectedEventArgs<Service1>> Service1Connected = delegate {};
		public event EventHandler<ServiceConnectedEventArgs<Service2>> Service2Connected = delegate {};

		// declarations
		public readonly bool RestartMainActivityOnCrash = false;
		protected readonly int totalInitCount = 2;
		protected int currentInitCount = 0;
		protected readonly string logTag = "App";
		protected object locker = new object();
		protected ServiceConnection<Service1> service1Connection;
		protected ServiceConnection<Service2> service2Connection;

		// properties
		public bool IsInitialized { get; set; }

		public static App Current
		{
			get { return current; }
		} private static App current;

		public Service1 Service1
		{
			get {
				if (this.service1Connection == null)
					return null;
				if (this.service1Connection.Binder == null)
					return null;
				return this.service1Connection.Binder.Service;
			}
		}
		
		public Service2 Service2
		{
			get {
				if (this.service2Connection == null)
					return null;
				if (this.service2Connection.Binder == null)
					return null;
				return this.service2Connection.Binder.Service;
			}
		}

		static App ()
		{
			current = new App();
		}
		protected App () 
		{
			Log.Debug (logTag, "Initializing App class");

			// subscribe to app wide unhandled exceptions so that we can log them.
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

			// any work here is likely to be blocking (static constructors run on whatever thread that first 
			// access its instance members, which in our case is an activity doing an initialization check),
			// so we want to do it on a background thread
			new Task ( () => { 
				
				// add a little wait time, to illustrate a loading event
				Thread.Sleep ( 2500 );
				
				// start our services
				Log.Debug (logTag, "Calling StartService");
				Android.App.Application.Context.StartService ( new Intent ( Android.App.Application.Context, typeof(Service1) ) );
				Android.App.Application.Context.StartService ( new Intent ( Android.App.Application.Context, typeof(Service2) ) );

				// create a new service connection so we can get a binder to the service
				this.service1Connection = new ServiceConnection<Service1> (null);
				this.service1Connection.ServiceConnected += (object sender, ServiceConnectedEventArgs<Service1> e) => {
					this.Service1Connected ( this, e );
					// increment our initialization count
					this.IncrementInitCount();
					Log.Debug (logTag, "Service1 Connected");
				};
				this.service2Connection = new ServiceConnection<Service2> (null);
				this.service2Connection.ServiceConnected += (object sender, ServiceConnectedEventArgs<Service2> e) => {
					this.Service2Connected ( this, e );
					// increment our initialization count
					this.IncrementInitCount();
					Log.Debug (logTag, "Service2 Connected");
				};

				// bind our services
				Intent service1Intent = new Intent (Android.App.Application.Context, typeof(Service1));
				Intent service2Intent = new Intent (Android.App.Application.Context, typeof(Service2));
				Log.Debug (logTag, "Calling BindService");
				Android.App.Application.Context.BindService ( service1Intent, service1Connection, Bind.AutoCreate );
				Android.App.Application.Context.BindService ( service2Intent, service2Connection, Bind.AutoCreate );


			} ).Start ();
		}

		protected void IncrementInitCount()
		{
			lock (this.locker) {
				Log.Debug (logTag, "App.IncrementInitCount");
				this.currentInitCount++;
				if (this.currentInitCount == this.totalInitCount) {
					this.IsInitialized = true;
					// raise our intialized event
					this.Initialized (this, new EventArgs ());
				}
			}
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

