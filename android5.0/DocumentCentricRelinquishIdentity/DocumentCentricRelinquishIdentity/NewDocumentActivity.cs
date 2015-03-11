
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Graphics;

namespace DocumentCentricRelinquishIdentity
{
	[Activity (Label = "NewDocumentActivity")]			
	public class NewDocumentActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_document);
			SetTaskDescription (NewTaskDescription ());
		}

		/**
		 * Creates a new ActivityManager.TaskDescription witha  new label and icon to change the 
		 * appearance of this activity in the recents stack.
		 */ 
		private ActivityManager.TaskDescription NewTaskDescription()
		{
			Bitmap newIcon = BitmapFactory.DecodeResource (Resources,Resource.Drawable.new_icon);
			string newDocumentRecentsLabel = GetString (Resource.String.new_document_recents_label);
			var newTaskDescription = new ActivityManager.TaskDescription (newDocumentRecentsLabel, newIcon);
			return newTaskDescription;
		}
	}
}

