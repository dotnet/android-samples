using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;
using Android.Text;

namespace ElizaChat
{
	[Activity (Label = "ElizaChat", MainLauncher = true, Icon = "@drawable/ic_app_eliza")]
	public class MainActivity : Activity
	{
		private static readonly string TAG = "MainActivity";

		public static string EXTRA_MESSAGE = "message";

		public const string  ACTION_NOTIFY = "com.example.android.wearable.elizachat.NOTIFY";

		public const string ACTION_GET_CONVERSATION = "com.example.android.wearable.elizachat.CONVERSATION";

		private Receiver mReceiver;

		private TextView mHistoryView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);
			mReceiver = new Receiver (this);
			mHistoryView = FindViewById<TextView> (Resource.Id.history);
			StartResponderService (ResponderService.ACTION_INCOMING);
		}

		private void StartResponderService (string action)
		{
			Intent serviceIntent = new Intent(this, typeof(ResponderService));
			serviceIntent.SetAction(action);
			this.StartService (serviceIntent);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			LocalBroadcastManager.GetInstance (this).RegisterReceiver (mReceiver, new IntentFilter (ACTION_NOTIFY));
			mHistoryView.Text = "";
			StartResponderService(ACTION_GET_CONVERSATION);
		}

		protected override void OnPause ()
		{
			LocalBroadcastManager.GetInstance (this).RegisterReceiver (mReceiver, new IntentFilter (ACTION_NOTIFY));
			base.OnPause ();
		}


		public void ProcessMessage (Intent intent)
		{
			var text = intent.GetStringExtra (EXTRA_MESSAGE);
			if (!TextUtils.IsEmpty (text))
				mHistoryView.Append ("\n" + text);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.action_stop_service:
				StopService (new Intent (this, typeof(ResponderService)));
				Finish ();
				break;
			}
			return true;
		}

		internal class Receiver : BroadcastReceiver
		{
			MainActivity owner;

			public Receiver (MainActivity owner)
			{
				this.owner = owner;
			}

			public override void OnReceive (Context context, Intent intent)
			{
				owner.ProcessMessage (intent);
			}

		}
	}
}


