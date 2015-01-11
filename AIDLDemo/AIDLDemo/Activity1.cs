using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Widget;
using Android.OS;
using Com.Xamarin.Aidldemo;

namespace Xamarin.AidlDemo
{
	[Activity (Label = "AIDL Demo", MainLauncher = true)]
	public class Activity1 : Activity
	{
		public static readonly String Tag = "Activity1";
		private AdditionServiceConnection _serviceConnection;

		public IAdditionService Service { get; set; }

		public bool IsBound { get; set; }

		protected override void OnStart ()
		{
			base.OnStart ();
			InitService ();

			var button1 = FindViewById<Button> (Resource.Id.buttonCalc);
			button1.Click += (sender, e) => {
				if (IsBound) {
					var text1 = FindViewById<EditText> (Resource.Id.value1);
					var text2 = FindViewById<EditText> (Resource.Id.value2);
					var result = FindViewById<TextView> (Resource.Id.result);

					int v1;
					int v2;
					int v3;

					if(Int32.TryParse (text2.Text, out v2) && Int32.TryParse (text1.Text, out v1)) {
						v3 = Service.Add (v1, v2);
					} else {
						v3 = 0;
						var builder = new AlertDialog.Builder(this);
						builder.SetMessage("Spaces or special character are not allowed");
						builder.SetNeutralButton("OK", (source, eventArgs) => {});
						builder.Show();
					}

					result.Text = v3.ToString ();
				} else {
					Log.Warn (Tag, "The AdditionService is not bound");
				}

			};

		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main);
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			ReleaseService ();
		}

		private void InitService ()
		{
			_serviceConnection = new AdditionServiceConnection (this);
			var additionServiceIntent = new Intent ("com.xamarin.additionservice");
			_serviceConnection = new AdditionServiceConnection (this);
			ApplicationContext.BindService (additionServiceIntent, _serviceConnection, Bind.AutoCreate);
			Log.Debug (Tag, "Service initialized");
		}

		private void ReleaseService ()
		{
			if (IsBound) {
				ApplicationContext.UnbindService (_serviceConnection);
				IsBound = false;
				_serviceConnection = null;
				Log.Debug (Tag, "Service released.");
			}
		}
	}
}


