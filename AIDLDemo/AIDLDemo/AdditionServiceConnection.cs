
using Android.Content;
using Android.OS;
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

		public IAdditionService? Service 
		{
			get; private set;
		}

		public void OnServiceConnected (ComponentName? name, IBinder? service)
		{
			ArgumentNullException.ThrowIfNull(service);
			Service =   IAdditionServiceStub.AsInterface(service);
			_activity.Service = (IAdditionService) Service;
		}

		public void OnServiceDisconnected (ComponentName? name)
		{
			_activity.Service = null;
		}
	}
}

