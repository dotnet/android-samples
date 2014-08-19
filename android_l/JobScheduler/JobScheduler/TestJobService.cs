using System;
using System.Collections.Generic;
using Android.App.Job;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

using JobSchedulerType = Android.App.Job.JobScheduler;

namespace JobScheduler
{
	[Service (Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
	public class TestJobService : JobService
	{
		private static string Tag = "SyncService";
		MainActivity owner;
		private readonly List<JobParameters> jobParamsMap = new List<JobParameters> ();

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Info (Tag, "Service created");
		}

		public override void OnDestroy ()
		{
			Log.Info (Tag, "Service destroyed");
			base.OnDestroy ();
		}

		public override StartCommandResult OnStartCommand (Intent intent, Android.App.StartCommandFlags flags, int startId)
		{
			var callback = (Messenger)intent.GetParcelableExtra ("messenger");
			var m = Message.Obtain ();
			m.What = MainActivity.MessageServiceObj;
			m.Obj = this;
			try {
				callback.Send (m);
			} catch (RemoteException e) {
				Log.Error (Tag, e, "Error passing service object back to activity.");
			}
			return StartCommandResult.NotSticky;
		}

		public override bool OnStartJob (JobParameters args)
		{
			// We don't do any real 'work' in this sample app. All we'll
			// do is track which jobs have landed on our service, and
			// update the UI accordingly.
			jobParamsMap.Add (args);
			if (owner != null) {
				owner.OnReceivedStartJob (args);
			}
			Log.Info (Tag, "on start job: " + args.JobId);
			return true;
		}

		public override bool OnStopJob (JobParameters args)
		{
			// Stop tracking these job parameters, as we've 'finished' executing.
			jobParamsMap.Remove (args);
			if (owner != null) {
				owner.OnReceivedStopJob ();
			}
			Log.Info (Tag, "on stop job: " + args.JobId);
			return true;
		}

		public void setUiCallback (MainActivity activity)
		{
			owner = activity;
		}

		/** Send job to the JobScheduler. */
		public void ScheduleJob (JobInfo t)
		{
			var tm = (JobSchedulerType)GetSystemService (Context.JobSchedulerService);
			var status = tm.Schedule (t);
			Log.Info (Tag, "Scheduling job: " + (status == JobSchedulerType.ResultSuccess ? "Success" : "Failure"));
		}

		/**
	     * Called when Task Finished button is pressed. 
	     */
		public bool CallJobFinished ()
		{
			if (jobParamsMap.Count == 0) {
				return false;
			} else {
				var args = jobParamsMap [0];
				jobParamsMap.Remove (args);
				JobFinished (args, false);
				return true;
			}
		}
	}
}