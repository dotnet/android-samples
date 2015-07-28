using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Widget;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Graphics;

using CommonSampleLibrary;


namespace ClippingBasic
{
	public class ClippingBasicFragment : Fragment
	{
		static readonly string TAG = "ClippingBasicFragment";

		/* Store the click count so that we can show a different text on every click. */
		int clickCount = 0;

		/* The {@Link Outline} used to clip the image with. */
		ClipOutlineProvider outlineProvider;

		/* An array of texts. */
		String[] sampleTexts;

		/* A reference to a {@Link TextView} that shows different text strings when clicked. */
		TextView textView;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			outlineProvider = new ClipOutlineProvider ();
			sampleTexts = Resources.GetStringArray (Resource.Array.sample_texts);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.clipping_basic_fragment, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			/* Set the initial text for the TextView. */
			textView = view.FindViewById<TextView> (Resource.Id.text_view);
			ChangeText ();

			View clippedView = view.FindViewById (Resource.Id.frame);

			/* Sets the OutlineProvider for the View. */
			clippedView.OutlineProvider = outlineProvider;

			/* When the button is clicked, the text is clipped or un-clipped. */
			var clipButton = view.FindViewById <Button> (Resource.Id.button);
			clipButton.Click += delegate {
				if (clippedView.ClipToOutline) {
					/* The Outline is set for the View, but disable clipping. */
					clippedView.ClipToOutline = false;
					Log.Debug (TAG, "Clipping to outline is disabled");
					clipButton.Text = GetString (Resource.String.clip_button);
				} else {
					/* Enables clipping on the View. */
					clippedView.ClipToOutline = true;
					Log.Debug (TAG, "Clipping to outline is enabled");
					clipButton.Text = GetString (Resource.String.unclip_button);
				}
			};

			/* When the text is clicked, a new string is shown. */
			textView.Click += delegate {
				clickCount++;

				// Update the text in the TextView
				ChangeText ();

				// Invalidate the outline just in case the TextView changed size
				clippedView.InvalidateOutline ();
			};
		}
			
		void ChangeText () 
		{
			// Compute the position of the string in the array using the number of strings
			//  and the number of clicks.
			string newText = sampleTexts[clickCount % sampleTexts.Length];

			/* Once the text is selected, change the TextView */
			textView.Text = newText;
			Log.Debug (TAG, "Text was changed.");
		}
	}

	public class ClipOutlineProvider : ViewOutlineProvider
	{
		public override void GetOutline (View view, Outline outline)
		{
			int margin = Math.Min (view.Width, view.Height) / 10;

			outline.SetRoundRect (margin, margin, view.Width - margin,
				view.Height - margin, margin / 2);
		}
	}
}

