using System;
using Android.OS;
using Android.App;

namespace AppLifecycle.Services
{
	public class ServiceConnectedEventArgs<T> : EventArgs where T : Service
	{
		public ServiceBinder<T> Binder { get; set; }
	}
}