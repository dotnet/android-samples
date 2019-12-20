using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.LocalBroadcastManager.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;

namespace DirectBoot
{
	/**
 	* Fragment that registers scheduled alarms.
 	*/
	public class SchedulerFragment : AndroidX.Fragment.App.Fragment
	{
		static readonly string FRAGMENT_TIME_PICKER_TAG = "fragment_time_picker";
		public AlarmAdapter AlarmAdapter { get; set; }
		public AlarmUtil AlarmUtil { get; set; }
		public TextView TextViewIntroMessage { get; set; }
		BroadcastReceiver alarmWentOffBroadcastReceiver;

		public static SchedulerFragment NewInstance ()
		{
			return new SchedulerFragment ();
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);

			alarmWentOffBroadcastReceiver = new AlarmWentOffReceiver (this);
			LocalBroadcastManager.GetInstance (Activity)
								 .RegisterReceiver (alarmWentOffBroadcastReceiver,
													new IntentFilter (AlarmIntentService.ALARM_WENT_OFF_ACTION));
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_alarm_scheduler, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			var fab = (FloatingActionButton)view.FindViewById (Resource.Id.fab_add_alarm);
			fab.Click += delegate {
				var fragment = TimePickerFragment.NewInstance ();
				fragment.SetAlarmAddListener (new AlarmAddListenerImpl (this));
				fragment.Show (FragmentManager, FRAGMENT_TIME_PICKER_TAG);
			};

			TextViewIntroMessage = (TextView)view.FindViewById (Resource.Id.text_intro_message);
			var alarmStorage = new AlarmStorage (Activity);
			AlarmAdapter = new AlarmAdapter (Activity, alarmStorage.GetAlarms ());

			if (AlarmAdapter.ItemCount == 0)
				TextViewIntroMessage.Visibility = ViewStates.Visible;

			var recyclerView = (RecyclerView)view.FindViewById (Resource.Id.recycler_view_alarms);
			recyclerView.SetLayoutManager (new LinearLayoutManager (Activity));
			recyclerView.SetAdapter (AlarmAdapter);
			recyclerView.AddItemDecoration (new DividerItemDecorrection (Activity));
			AlarmUtil = new AlarmUtil (Activity);
		}

		public override void OnDestroy ()
		{
			LocalBroadcastManager.GetInstance (Activity).UnregisterReceiver (alarmWentOffBroadcastReceiver);
			base.OnDestroy ();
		}
	}

	public class AlarmAddListenerImpl : Java.Lang.Object, IAlarmAddListener
	{
		SchedulerFragment parentFragment;

		public AlarmAddListenerImpl (SchedulerFragment parent)
		{
			parentFragment = parent;
		}

		public void OnAlarmAdded (Alarm alarm)
		{
			parentFragment.AlarmAdapter.AddAlarm (alarm);
			parentFragment.AlarmUtil.ScheduleAlarm (alarm);
			parentFragment.TextViewIntroMessage.Visibility = ViewStates.Gone;
		}
	}


	[BroadcastReceiver]
	public class AlarmWentOffReceiver : BroadcastReceiver
	{
		SchedulerFragment parentFragment;

		public AlarmWentOffReceiver () { }

		public AlarmWentOffReceiver (SchedulerFragment parent)
		{
			parentFragment = parent;
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var alarm = (Alarm)intent.GetParcelableExtra (AlarmIntentService.ALARM_KEY);
			parentFragment.AlarmAdapter.DeleteAlarm (alarm);
		}
	}
}

