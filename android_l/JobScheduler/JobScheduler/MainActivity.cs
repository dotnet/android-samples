using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App.Job;
using Android.Text;
using Android.Graphics;

using JobSchedulerType = Android.App.Job.JobScheduler;
using Export = Java.Interop.ExportAttribute;

namespace JobScheduler
{
	[Activity (Label = "JobScheduler", MainLauncher = true, Icon = "@drawable/ic_launcher", WindowSoftInputMode = SoftInput.StateHidden)]
	public class MainActivity : Activity
	{
		//Constants
		private const string Tag = "MainActivity";
		public const int MessageUncolorStart = 0;
		public const int MessageUncolorStop = 1;
		public const int MessageServiceObj = 2;
		public int kJobId = 0;

		Color defaultColor;
		Color startJobColor;
		Color stopJobColor;

		private TextView showStartView;
		private TextView showStopView;
		private TextView paramsTextView;
		private EditText delayEditText;
		private EditText deadlineEditText;
		private RadioButton wiFiConnectivityRadioButton;
		private RadioButton anyConnectivityRadioButton;
		private CheckBox requiresChargingCheckBox;
		private CheckBox requiresIdleCheckbox;

		ComponentName serviceComponent;
		/** Service object to interact scheduled jobs. */
		TestJobService testService;

		Handler handler;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			handler = new Handler ((Message msg) => {
				switch (msg.What) {
				case MessageUncolorStart:
					showStartView.SetBackgroundColor (defaultColor);
					break;
				case MessageUncolorStop:
					showStopView.SetBackgroundColor (defaultColor);
					break;
				case MessageServiceObj:
					testService = (TestJobService)msg.Obj;
					testService.setUiCallback (this);
					break;
				}
			});

			SetContentView (Resource.Layout.sample_main);
			var res = this.Resources;
			defaultColor = res.GetColor (Resource.Color.none_received);
			startJobColor = res.GetColor (Resource.Color.start_received);
			stopJobColor = res.GetColor (Resource.Color.stop_received);

			// Set up UI.
			showStartView = FindViewById<TextView> (Resource.Id.onstart_textview);
			showStopView = FindViewById<TextView> (Resource.Id.onstop_textview);
			paramsTextView = FindViewById <TextView> (Resource.Id.task_params);
			delayEditText = FindViewById <EditText> (Resource.Id.delay_time);
			deadlineEditText = FindViewById <EditText> (Resource.Id.deadline_time);
			wiFiConnectivityRadioButton = FindViewById <RadioButton> (Resource.Id.checkbox_unmetered);
			anyConnectivityRadioButton = FindViewById <RadioButton> (Resource.Id.checkbox_any);
			requiresChargingCheckBox = FindViewById <CheckBox> (Resource.Id.checkbox_charging);
			requiresIdleCheckbox = FindViewById <CheckBox> (Resource.Id.checkbox_idle);
			serviceComponent = new ComponentName (this, Java.Lang.Class.FromType (typeof(TestJobService)));
			// Start service and provide it a way to communicate with us.
			var startServiceIntent = new Intent (this, typeof(TestJobService));
			startServiceIntent.PutExtra ("messenger", new Messenger (handler));
			StartService (startServiceIntent);
		}

		private bool EnsureTestService ()
		{
			if (testService == null) {
				Toast.MakeText (this, "Service null, never got callback?",
					ToastLength.Short).Show ();
				return false;
			}
			return true;
		}

		/**
	     * UI onclick listener to schedule a job. What this job is is defined in
	     * TestJobService#scheduleJob().
	     */
		[Export ("scheduleJob")]
		public void ScheduleJob (View v)
		{
			if (!EnsureTestService ()) {
				return;
			}
			var builder = new JobInfo.Builder (kJobId++, serviceComponent);

			var delay = delayEditText.Text;
			if (delay != null && !TextUtils.IsEmpty (delay)) {
				builder.SetMinimumLatency (long.Parse (delay) * 1000);
			}
			var deadline = deadlineEditText.Text;
			if (deadline != null && !TextUtils.IsEmpty (deadline)) {
				builder.SetOverrideDeadline (long.Parse (deadline) * 1000);
			}
			bool requiresUnmetered = wiFiConnectivityRadioButton.Checked;
			bool requiresAnyConnectivity = anyConnectivityRadioButton.Checked;
			if (requiresUnmetered) {
				builder.SetRequiredNetworkType (NetworkType.Unmetered);
			} else if (requiresAnyConnectivity) {
				builder.SetRequiredNetworkType (NetworkType.Any);
			}
			builder.SetRequiresDeviceIdle (requiresIdleCheckbox.Checked);
			builder.SetRequiresCharging (requiresChargingCheckBox.Checked);

			testService.ScheduleJob (builder.Build ());
		}

		[Export ("cancelAllJobs")]
		public void CancelAllJobs (View v)
		{
			var tm = (JobSchedulerType)GetSystemService (Context.JobSchedulerService);
			tm.CancelAll ();
		}

		/**
	     * UI onclick listener to call jobFinished() in our service.
	     */
		[Export ("finishJob")]
		public void FinishJob (View v)
		{
			if (!EnsureTestService ()) {
				return;
			}
			testService.CallJobFinished ();
			paramsTextView.Text = "";
		}

		/**
	     * Receives callback from the service when a job has landed
	     * on the app. Colours the UI and post a message to
	     * uncolour it after a second.
	     */
		public void OnReceivedStartJob (JobParameters args)
		{
			showStartView.SetBackgroundColor (startJobColor);
			var m = Message.Obtain (handler, MessageUncolorStart);
			handler.SendMessageDelayed (m, 1000L); // uncolour in 1 second.
			paramsTextView.Text = ("Executing: " + args.JobId + " " + args.Extras);
		}

		/**
	     * Receives callback from the service when a job that
	     * previously landed on the app must stop executing.
	     * Colours the UI and post a message to uncolour it after a
	     * second.
	     */
		public void OnReceivedStopJob ()
		{
			showStopView.SetBackgroundColor (stopJobColor);
			var m = Message.Obtain (handler, MessageUncolorStop);
			handler.SendMessageDelayed (m, 1000L); // uncolour in 1 second.
			paramsTextView.Text = "";
		}
	}
}