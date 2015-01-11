using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace AppLifecycle.Services
{
	/// <summary>
	/// Responsible for creating and monitoring a connection to a service (via a Binder).
	/// </summary>
	public class ServiceConnection<T> : Java.Lang.Object, IServiceConnection where T : Service
	{
		/// <summary>
		/// Occurs when service has been connected.
		/// </summary>
		public event EventHandler<ServiceConnectedEventArgs<T>> ServiceConnected = delegate {};

		public ServiceBinder<T> Binder
		{
			get { return this.binder; }
			set { this.binder = value; }
		}
		protected ServiceBinder<T> binder;

		public ServiceConnection (ServiceBinder<T> binder)
		{
			if (binder != null) {
				this.binder = binder;
			}
		}

		/// <summary>
		/// Called when a connection to the service has been established
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="service">Service.</param>
		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			ServiceBinder<T> serviceBinder = service as ServiceBinder<T>;
			if (serviceBinder != null) {
				// save the reference to the binder and flag as Bound
				this.binder = serviceBinder;
				this.binder.IsBound = true;
				Log.Debug ( "ServiceConnection", "OnServiceConnected Called" );

				// raise the service bound event
				this.ServiceConnected(this, new ServiceConnectedEventArgs<T> () { Binder = serviceBinder } );
			}
		}

		/// <summary>
		/// Called when the app unbinds from the service.
		/// </summary>
		/// <param name="name">Name.</param>
		public void OnServiceDisconnected (ComponentName name)
		{
			this.binder.IsBound = false;
		}
	}
}