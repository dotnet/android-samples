using System;
using Android.OS;
using Android.App;

namespace AppLifecycle.Services
{
	/// <summary>
	/// A binder is an object that android uses to be the pipe between apps and remotable objects.
	/// In this case, the binder is for our MainAppService class. 
	/// </summary>
	public class ServiceBinder<T> : Binder where T : Service
	{
		/// <summary>
		/// Gets a reference to the Service
		/// </summary>
		/// <value>The service.</value>
		public T Service
		{
			get { return this.service; }
		}
		protected T service;

		/// <summary>
		/// Whether or not the service has been connected/bound.
		/// </summary>
		/// <value><c>true</c> if this instance is bound; otherwise, <c>false</c>.</value>
		public bool IsBound { get; set; }

		public ServiceBinder (T service)
		{
			this.service = service;
		}
	}
}