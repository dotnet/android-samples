using Android.Content;
using Android.OS;
using Android.Util;

namespace Xamarin.AidlDemo;

[Service(Exported = true)]
[IntentFilter(new String[] {"com.xamarin.additionservice"})]
public class AdditionService : Service
{
	private static readonly string Tag = "AdditionService";
	private AdditionServiceBinder? _binder;

	public override void OnCreate ()
	{
		base.OnCreate ();
		Log.Debug (Tag, "Addition Service created.");
	}

	public override IBinder OnBind (Intent? intent)
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
