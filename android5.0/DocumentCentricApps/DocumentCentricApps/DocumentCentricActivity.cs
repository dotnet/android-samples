using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace DocumentCentricApps
{
	/**
	 * DocumentCentricActivity shows the basic usage of the new Document-Centric Apps API. The new
	 * API modifies the meaning of the ActivityFlags.ClearWhenTaskReset flag, which is
	 * now deprecated. In versions before L it serves to define a boundary between the main task and a
	 * subtask. The subtask holds a different thumbnail and all activities in it are finished when the
	 * task is reset. In L this flag causes a full break with the task that launched it. As such it has
	 * been renamed to ActivityFlags.NewDocument}.
	 *
	 * This sample mainly uses Intent flags in code. But Activities can also specify in their manifests
	 * that they shall always be launched into a new task in the above manner using the new activity
	 * attribute documentLaunchMode which may take on one of three values, “intoExisting” equivalent to
	 * NewDocument, “always” equivalent to NewDocument | MultipleTask, “none” the default, and
	 * “never” which will negate the effect of any attempt to launch the activity with NewDocument.
	 */
	[Activity (Label = "DocumentCentricApps", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class DocumentCentricActivity: Activity
	{
		public const string TAG = "DocumentCentricActivity";
		public const string KEY_EXTRA_NEW_DOCUMENT_COUNTER = "KEY_EXTRA_NEW_DOCUMENT_COUNTER";
		private static int documentCounter = 0;
		private CheckBox checkBox;
		private Button createDocument;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_document_centric_main);
			checkBox = FindViewById<CheckBox> (Resource.Id.multiple_task_checkbox);
			createDocument = FindViewById<Button> (Resource.Id.new_document_button);
			createDocument.Click += delegate {
				CreateNewDocument ();
			};
		}

		public override void OnPostCreate (Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnPostCreate (savedInstanceState, persistentState);

			// Restore state from PersistableBundle
			if (persistentState != null)
				documentCounter = persistentState.GetInt (KEY_EXTRA_NEW_DOCUMENT_COUNTER);
		}

		public override void OnSaveInstanceState (Bundle outState, PersistableBundle outPersistentState)
		{
			/*
	        To maintain activity state across reboots the system saves and restore critical information for
	        all tasks and their activities. Information known by the system includes the activity stack order,
	        each task’s thumbnails and each activity’s and task's Intents. For Information that cannot be retained
	        because they contain Bundles which can’t be persisted a new constrained version of Bundle,
	        PersistableBundle is added. PersistableBundle can store only basic data types. To use it
	        in your Activities you must declare the new activity:persistableMode attribute in the manifest.
	         */
			outPersistentState.PutInt (KEY_EXTRA_NEW_DOCUMENT_COUNTER, documentCounter);
			base.OnSaveInstanceState (outState, outPersistentState);
		}

		public void CreateNewDocument()
		{
			bool useMultipleTasks = checkBox.Checked;
			Intent documentIntent = NewDocumentIntent ();
			if (useMultipleTasks) {
				/*
	            When ActivityFlags.NewDocument is used with ActivityFlags.MultipleTask
	            the system will always create a new task with the target activity as the root. This allows the same
	            document to be opened in more than one task.
	             */
				documentIntent.AddFlags (ActivityFlags.MultipleTask);
			}
			StartActivity (documentIntent);

		}

		/**
	     * Returns an new Intent to start NewDocumentActivity as a new document in
	     * overview menu.
	     *
	     * To start a new document task ActivityFlags.NewDocument must be used. The
	     * system will search through existing tasks for one whose Intent matches the Intent component
	     * name and the Intent data. If it finds one then that task will be brought to the front and the
	     * new Intent will be passed to onNewIntent().
	     *
	     * Activities launched with the NewDocument flag must be created with launchMode="standard".
	     */
		private Intent NewDocumentIntent()
		{
			Intent newDocumentIntent = new Intent (this, Java.Lang.Class.FromType (typeof(NewDocumentActivity)));
			newDocumentIntent.AddFlags (ActivityFlags.NewDocument);
			newDocumentIntent.PutExtra (KEY_EXTRA_NEW_DOCUMENT_COUNTER, IncrementAndGet ());
			return newDocumentIntent;

		}

		private static int IncrementAndGet()
		{
			Log.Debug (TAG, "incrementAndGet(): " + documentCounter);
			return documentCounter++;
		}
	}
}


