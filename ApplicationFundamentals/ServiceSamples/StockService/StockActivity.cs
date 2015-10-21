using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace StockService
{
	[Activity (Label = "StockService", MainLauncher = true, Icon="@drawable/Icon")]
	public class StockActivity : ListActivity
	{
		bool isBound = false;
		StockServiceBinder binder;
		StockServiceConnection stockServiceConnection;
		StockReceiver stockReceiver;
		Intent stockServiceIntent;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			stockServiceIntent = new Intent ("com.xamarin.StockService");
			stockReceiver = new StockReceiver ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			var intentFilter = new IntentFilter (StockService.StocksUpdatedAction){Priority = (int)IntentFilterPriority.HighPriority};
			RegisterReceiver (stockReceiver, intentFilter);

			stockServiceConnection = new StockServiceConnection (this);
			BindService (stockServiceIntent, stockServiceConnection, Bind.AutoCreate);

			ScheduleStockUpdates ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();

			if (isBound) {
				UnbindService (stockServiceConnection);

				isBound = false;
			}

			UnregisterReceiver (stockReceiver);
		}

		void ScheduleStockUpdates ()
		{
			if (!IsAlarmSet ()) {
				var alarm = (AlarmManager)GetSystemService (Context.AlarmService);

				var pendingServiceIntent = PendingIntent.GetService (this, 0, stockServiceIntent, PendingIntentFlags.CancelCurrent);
				alarm.SetRepeating (AlarmType.Rtc, 0, 5000, pendingServiceIntent);
				//alarm.SetRepeating (AlarmType.Rtc, 0, AlarmManager.IntervalHalfHour, pendingServiceIntent);
			} else {
				Console.WriteLine ("alarm already set");
			}
		}

		bool IsAlarmSet ()
		{
			return PendingIntent.GetBroadcast (this, 0, stockServiceIntent, PendingIntentFlags.NoCreate) != null;
		}

		void GetStocks ()
		{
			if (isBound) {
				RunOnUiThread (() => {
					var stocks = binder.GetStockService ().GetStocks ();

					if (stocks != null) {
						ListAdapter = new ArrayAdapter<Stock> (
                    	this,
                     	Resource.Layout.StockItemView,
                        stocks
						); 
					} else {
						Log.Debug ("StockService", "stocks is null");
					}
				}
				);
			}
		}

		class StockReceiver : BroadcastReceiver
		{
			public override void OnReceive (Context context, Android.Content.Intent intent)
			{
				((StockActivity)context).GetStocks ();

				InvokeAbortBroadcast ();
			}
		}

		class StockServiceConnection : Java.Lang.Object, IServiceConnection
		{
			StockActivity activity;

			public StockServiceConnection (StockActivity activity)
			{
				this.activity = activity;
			}
          
			public void OnServiceConnected (ComponentName name, IBinder service)
			{
				var stockServiceBinder = service as StockServiceBinder;
				if (stockServiceBinder != null) {
					var binder = (StockServiceBinder)service;
					activity.binder = binder;
					activity.isBound = true;
				}
			}

			public void OnServiceDisconnected (ComponentName name)
			{
				activity.isBound = false;
			}
		}
	}
}


