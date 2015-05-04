using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V7.Widget;

namespace Timer
{
	[Activity (Label = "Timer", MainLauncher = true, Icon = "@mipmap/ic_launcher")]

	[IntentFilter (new[]{ "com.android.example.clockwork.timer.TIMER" }, Categories = new[]{ "android.intent.category.DEFAULT" })]
	//Intent filter for the Android voice command
	[IntentFilter (new[] { "android.intent.action.SET_TIMER" }, Categories = new[]{ "android.intent.category.DEFAULT" })]
	public class SetTimerActivity : Activity,  WearableListView.IClickListener
	{
		public const int NumberOfTimes = 10;
		public const string Tag = "SetTimerActivity";

		private ListViewItem[] timeOptions = new ListViewItem[NumberOfTimes];
		private WearableListView wearableListView;


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			int paramLength = this.Intent.GetIntExtra (AlarmClock.ExtraLength, 0);

			if (Log.IsLoggable (Tag, LogPriority.Debug))
				Log.Debug (Tag, "SetTimerActivity:onCreate=" + paramLength);
			
			if (paramLength > 0 && paramLength <= 86400) {
				long durationMillis = paramLength * 1000;
				SetupTimer (durationMillis);
				Finish ();
				return;
			}

			var res = this.Resources;
			for (int i = 0; i < NumberOfTimes; i++) {
				timeOptions [i] = new ListViewItem (
					res.GetQuantityString (Resource.Plurals.timer_minutes, i + 1, i + 1),
					(i + 1) * 60 * 1000);
			}

			SetContentView (Resource.Layout.timer_set_timer);

			// Initialize a simple list of countdown time options.
			wearableListView = FindViewById<WearableListView> (Resource.Id.times_list_view);
			wearableListView.SetAdapter (new TimerWearableListViewAdapter (this));
			wearableListView.SetClickListener (this);
		}


		/**
	     * Sets up an alarm (and an associated notification) to go off after <code>duration</code>
	     * milliseconds.
	     */
		private void SetupTimer (long duration)
		{
			var notifyMgr = (NotificationManager)GetSystemService(NotificationService);

			// Delete dataItem and cancel a potential old countdown.
			CancelCountdown (notifyMgr);

			// Build notification and set it.
			notifyMgr.Notify (Constants.NOTIFICATION_TIMER_COUNTDOWN, BuildNotification (duration));

			// Register with the alarm manager to display a notification when the timer is done.
			RegisterWithAlarmManager (duration);

			Finish ();
		}

		public void OnClick (WearableListView.ViewHolder holder)
		{
			var duration = timeOptions [holder.Position].duration;
			SetupTimer (duration);

		}

		public void OnTopEmptyRegionClick ()
		{
			//Do nothing
		}

		private void RegisterWithAlarmManager (long duration)
		{
			// Get the alarm manager.
			var alarm = (AlarmManager)GetSystemService (AlarmService);

			// Create intent that gets fired when timer expires.
			var intent = new Intent (Constants.ACTION_SHOW_ALARM, null, this,
				             typeof(TimerNotificationService));
			var pendingIntent = PendingIntent.GetService (this, 0, intent,
				                    PendingIntentFlags.UpdateCurrent);

			// Calculate the time when it expires.
			long wakeupTime = Java.Lang.JavaSystem.CurrentTimeMillis () + duration;

			// Schedule an alarm.
			alarm.SetExact (AlarmType.RtcWakeup, wakeupTime, pendingIntent);
		}

		/**
	     * Build a notification including different actions and other various setup and return it.
	     *
	     * @param duration the duration of the timer.
	     * @return the notification to display.
	     */
		private Notification BuildNotification (long duration)
		{
			// Intent to restart a timer.
			var restartIntent = new Intent (Constants.ACTION_RESTART_ALARM, null, this,
				                    typeof(TimerNotificationService));
			var pendingIntentRestart = PendingIntent
				.GetService (this, 0, restartIntent, PendingIntentFlags.UpdateCurrent);

			// Intent to delete a timer.
			var deleteIntent = new Intent (Constants.ACTION_DELETE_ALARM, null, this,
				                   typeof(TimerNotificationService));
			var pendingIntentDelete = PendingIntent
				.GetService (this, 0, deleteIntent, PendingIntentFlags.UpdateCurrent);

			// Create countdown notification using a chronometer style.
			return new Notification.Builder (this)
				.SetSmallIcon (Resource.Drawable.ic_cc_alarm)
				.SetContentTitle (GetString (Resource.String.timer_time_left))
				.SetContentText (TimerFormat.GetTimeString (duration))
				.SetUsesChronometer (true)
				.SetWhen (Java.Lang.JavaSystem.CurrentTimeMillis () + duration)
				.AddAction (Resource.Drawable.ic_cc_alarm, GetString (Resource.String.timer_restart), pendingIntentRestart)
				.AddAction (Resource.Drawable.ic_cc_alarm, GetString (Resource.String.timer_delete), pendingIntentDelete)
				.SetDeleteIntent (pendingIntentDelete)
				.SetLocalOnly (true)
				.Build ();
		}

		/**
	     * Cancels an old countdown and deletes the dataItem.
	     *
	     * @param notifyMgr the notification manager.
	     */
		private void CancelCountdown (NotificationManager notifyManager)
		{
			notifyManager.Cancel (Constants.NOTIFICATION_TIMER_EXPIRED);
		}

		/** Model class for the listview. */
		internal class ListViewItem
		{

			// Duration in milliseconds.
			public long duration;
			// Label to display.
			public string label;

			public ListViewItem (string label, long duration)
			{
				this.label = label;
				this.duration = duration;
			}

			public override string ToString ()
			{
				return label;
			}
		}

		internal class TimerWearableListViewAdapter : RecyclerView.Adapter
		{
			private readonly SetTimerActivity owner;
			private readonly LayoutInflater mInflater;

			public TimerWearableListViewAdapter (SetTimerActivity owner)
			{
				this.owner = owner;
				mInflater = LayoutInflater.From (owner);
			}

			public override int ItemCount {
				get {
					return SetTimerActivity.NumberOfTimes;
				}
			}

			public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
			{
				return new WearableListView.ViewHolder (mInflater.Inflate (Resource.Layout.timer_list_item, null));
			}

			public override void OnBindViewHolder (Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
			{
				TextView view = holder.ItemView.FindViewById<TextView> (Resource.Id.time_text);
				view.Text = owner.timeOptions [position].label;
				holder.ItemView.Tag = position;
			}
		}
	}
}


