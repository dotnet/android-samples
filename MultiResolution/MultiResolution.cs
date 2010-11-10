using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;

namespace Mono.Samples.MultiResolution
{
	[Activity (Label = "Multi Resolution Demo", MainLauncher = true)]
	public class MultiResolution : Activity
	{
		private int photo_index = 0;

		private List<int> photo_ids = new List<int> () { Resource.drawable.sample_0,
			Resource.drawable.sample_1, Resource.drawable.sample_2, Resource.drawable.sample_3,
			Resource.drawable.sample_4, Resource.drawable.sample_5, Resource.drawable.sample_6,
			Resource.drawable.sample_7 };

		// Called when the activity is first created.
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.layout.main);
			ShowPhoto (photo_index);

			// Handle clicks on the 'Next' button.
			Button nextButton = FindViewById<Button> (Resource.id.next_button);

			nextButton.Click += delegate {
				photo_index = (photo_index + 1) % photo_ids.Count;
				ShowPhoto (photo_index);
			};
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt ("photo_index", photo_index);

			base.OnSaveInstanceState (outState);
		}

		protected override void OnRestoreInstanceState (Bundle outState)
		{
			photo_index = outState.GetInt ("photo_index");
			ShowPhoto (photo_index);

			base.OnRestoreInstanceState (outState);
		}

		private void ShowPhoto (int photoIndex)
		{
			ImageView imageView = FindViewById<ImageView> (Resource.id.image_view);
			imageView.SetImageResource (photo_ids[photoIndex]);

			TextView statusText = FindViewById<TextView> (Resource.id.status_text);
			statusText.Text = String.Format ("{0}/{1}", photoIndex + 1, photo_ids.Count);
		}
	}
}

