using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Widget;

namespace MonoDroid.ApiDemo {

	public class LocalServiceActivities {

		[Activity (Label = "@string/activity_local_service_controller")]
		[IntentFilter (new[] { Intent.ActionMain },
				Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
		public class Controller : Activity {

			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);

				SetContentView (Resource.Layout.local_service_controller);

				var button = FindViewById<Button>(Resource.Id.start);
				button.Click += delegate {
					StartService (new Intent (this, typeof (LocalService)));
				};
				button = FindViewById<Button>(Resource.Id.stop);
				button.Click += delegate {
					StopService (new Intent (this, typeof (LocalService)));
				};
			}
		}

		[Activity (Label = "@string/activity_local_service_binding")]
		[IntentFilter (new[] { Intent.ActionMain },
				Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
		public class Binding : Activity {

			bool isBound;
			LocalService boundService;
			IServiceConnection connection;

			public Binding ()
			{
				connection = new MyServiceConnection (this);
			}

			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);

				SetContentView (Resource.Layout.local_service_binding);

				var button = FindViewById<Button>(Resource.Id.bind);
				button.Click += delegate {
					BindService ();
				};
				button = FindViewById<Button>(Resource.Id.unbind);
				button.Click += delegate {
					UnbindService ();
				};
			}

			void BindService ()
			{
				base.BindService (new Intent (this, typeof (LocalService)),
							connection, Bind.AutoCreate);
				isBound = true;
			}

			void UnbindService ()
			{
				if (isBound) {
					base.UnbindService (connection);
					isBound = false;
				}
			}

			protected override void OnDestroy ()
			{
				base.OnDestroy ();
				UnbindService ();
			}

			class MyServiceConnection : Java.Lang.Object, IServiceConnection {
				Binding self;

				public MyServiceConnection (Binding self)
				{
					this.self = self;
				}

				public void OnServiceConnected (ComponentName className, IBinder service)
				{
					self.boundService = ((LocalService.LocalBinder) service).Service;
					Toast.MakeText (self, Resource.String.local_service_connected, ToastLength.Short).Show ();
				}

				public void OnServiceDisconnected (ComponentName className)
				{
					self.boundService = null;
					Toast.MakeText (self, Resource.String.local_service_disconnected, ToastLength.Short).Show ();
				}
			}
		}
	}
}

