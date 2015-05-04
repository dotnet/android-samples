using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace DocumentCentricRelinquishIdentity
{
	/**
	 * Activities that serve as the root of a task may give up certain task identifiers to activities
	 * above it in the task stack. These identifiers include the task base Intent, and the task name,
	 * color and icon used in the recent task list. The base Intent is used to match the task when
	 * relaunching based on an incoming Intent.
	 */ 
	[Activity (Label = "DocumentCentricRelinquishIdentity", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class RelinquishIdentityActivity : Activity
	{
		Button newDocument;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_relinquish_identity);
			newDocument = FindViewById<Button> (Resource.Id.new_document_button);
			newDocument.Click += delegate {
				CreateNewDocument ();
			};
		}

		public void CreateNewDocument()
		{
			Intent intent = NewDocumentIntent ();
			StartActivity (intent);
		}

		// Returns a new intent to start NewDocumentActivity as a new document in recents
		private Intent NewDocumentIntent()
		{
			var intent = new Intent (this, Java.Lang.Class.FromType (typeof(NewDocumentActivity)));
			intent.AddFlags (ActivityFlags.NewDocument);
			intent.AddFlags (ActivityFlags.MultipleTask);
			return intent;
		}
	}
}


