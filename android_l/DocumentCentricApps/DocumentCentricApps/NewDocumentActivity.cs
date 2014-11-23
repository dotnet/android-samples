
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DocumentCentricApps
{
	[Activity (Label = "NewDocumentActivity")]			
	public class NewDocumentActivity : Activity
	{
		private TextView documentCounterTextView;
		private int documentCount;
		private Button removeFromOverview;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_new_document);
			documentCount = Intent.GetIntExtra (DocumentCentricActivity.KEY_EXTRA_NEW_DOCUMENT_COUNTER, 0);
			documentCounterTextView = (TextView)FindViewById (Resource.Id.hello_new_document_text_view);
			removeFromOverview = FindViewById<Button> (Resource.Id.remove_task_button);
			removeFromOverview.Click+=delegate {
				OnRemoveFromOverview();
			};
			documentCounterTextView.SetText (string.Format ("Hello Document {0}!", documentCount),TextView.BufferType.Normal);
		}

		protected override void OnNewIntent (Intent intent)
		{
			base.OnNewIntent (intent);
			documentCounterTextView.SetText (string.Format ("Reusing Document {0}!", documentCount),TextView.BufferType.Normal);
		}

		public void OnRemoveFromOverview()
		{
			FinishAndRemoveTask ();
		}
	}
}

