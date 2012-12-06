
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
using Com.Xamarin.Aidldemo;

namespace Xamarin.AidlDemo
{
	class AdditionServiceConnection : Java.Lang.Object, IServiceConnection
	{
		Activity1 _activity;

		public AdditionServiceConnection (Activity1 activity)
		{
			_activity = activity;
		}

		public IAdditionService Service 
		{
			get; private set;
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			Service =   IAdditionServiceStub.AsInterface(service);
			_activity.Service = (IAdditionService) Service;
			_activity.IsBound = Service != null;

		}

		public void OnServiceDisconnected (ComponentName name)
		{
			_activity.Service = null;
			_activity.IsBound = false;
		}
	}
}

