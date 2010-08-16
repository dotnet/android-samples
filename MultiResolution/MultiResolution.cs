using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;

namespace Mono.Samples.MultiResolution
{
	public class MultiResolution : Activity
	{
		private int photo_index = 0;

		private List<int> photo_ids = new List<int> () { R.drawable.sample_0,
			R.drawable.sample_1, R.drawable.sample_2, R.drawable.sample_3,
			R.drawable.sample_4, R.drawable.sample_5, R.drawable.sample_6,
			R.drawable.sample_7 };

		public MultiResolution (IntPtr handle) : base (handle)
		{
		}

		// Called when the activity is first created.
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (R.layout.main);
			ShowPhoto (photo_index);

			// Handle clicks on the 'Next' button.
			Button nextButton = (Button)FindViewById (R.id.next_button);

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
			ImageView imageView = (ImageView)FindViewById (R.id.image_view);
			imageView.SetImageResource (photo_ids[photoIndex]);

			TextView statusText = (TextView)FindViewById (R.id.status_text);
			statusText.Text = String.Format ("{0}/{1}", photoIndex + 1, photo_ids.Count);
		}
	}
}

