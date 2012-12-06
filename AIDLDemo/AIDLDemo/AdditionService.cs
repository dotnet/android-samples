
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

namespace Xamarin.AidlDemo
{
	[Service]
	[IntentFilter(new String[] {"com.xamarin.additionservice"})]
	public class AdditionService: Service
	{
		private static readonly string Tag = "AdditionService";
		private AdditionServiceBinder _binder;
	
		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (Tag, "Addition Service created.");
		}

		public override IBinder OnBind (Intent intent)
		{
			_binder = new AdditionServiceBinder();
			return _binder;
		}
		public override void OnDestroy ()
		{
			base.OnDestroy ();
			Log.Debug (Tag, "Addition service stopped.");
		}

	}
}

