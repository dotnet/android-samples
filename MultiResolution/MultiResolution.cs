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

		private List<int> photo_ids = new List<int> () { Resource.Drawable.sample_0,
			Resource.Drawable.sample_1, Resource.Drawable.sample_2, Resource.Drawable.sample_3,
			Resource.Drawable.sample_4, Resource.Drawable.sample_5, Resource.Drawable.sample_6,
			Resource.Drawable.sample_7 };

		// Called when the activity is first created.
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.main);
			ShowPhoto (photo_index);

			// Handle clicks on the 'Next' button.
			Button nextButton = FindViewById<Button> (Resource.Id.next_button);

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
			ImageView imageView = FindViewById<ImageView> (Resource.Id.image_view);
			imageView.SetImageResource (photo_ids[photoIndex]);

			TextView statusText = FindViewById<TextView> (Resource.Id.status_text);
			statusText.Text = String.Format ("{0}/{1}", photoIndex + 1, photo_ids.Count);
		}
	}
}

