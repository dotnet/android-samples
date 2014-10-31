
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using Android.Content;
using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Graphics;
using CommonSampleLibrary;


namespace ClippingBasic
{
	public class ClippingBasicFragment : Fragment
	{
		const string TAG = "ClippingBasicFragment";

		// Store the click count so that we can show a different text on every click
		private int click_count=0;

		// The Outline used to clip the image
		private Outline clip;

		private string[] sample_texts;

		// A reference to a TextView that shows different strings when clicked
		private TextView text_view;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
			SetHasOptionsMenu (true);
			clip = new Outline ();
			sample_texts = Resources.GetStringArray (Resource.Array.sample_texts);
		}
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.clipping_basic_fragment, container, false);
		}
		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			// Sets initial text for the TextView
			text_view = (TextView)view.FindViewById (Resource.Id.text_view);
			ChangeText ();
			view.FindViewById (Resource.Id.button).SetOnClickListener (new ClippingListener (this));
			view.FindViewById (Resource.Id.text_view).SetOnClickListener (new TextListener (this));

		}

		private class ClippingListener : Java.Lang.Object,View.IOnClickListener
		{
			ClippingBasicFragment frag;
			public ClippingListener(ClippingBasicFragment f)
			{
				frag = f;
			}
			public void OnClick(View bt)
			{
				// When the button is clicked, the text is clipped or un-clipped
				View clippedView = frag.View.FindViewById (Resource.Id.frame);
				// If the view is clipped then ClipToOutline is true
				if (clippedView.ClipToOutline) {
					clippedView.ClipToOutline = false;

					Log.Debug (TAG, string.Format ("Clipping was removed."));
					(bt as Button).SetText (Resource.String.clip_button);
				} else {
					// If it is not clipped, it sets the dimensions and shapes of the clip and then clips the view.
					// In this case, it creates a rounded rectangle with a margin determined by width or height.
					int margin = Math.Min (clippedView.Width, clippedView.Height) / 10;
					frag.clip.SetRoundRect (margin, margin, clippedView.Width - margin,
						clippedView.Height - margin, margin / 2);
					// Sets the outline of the View
					clippedView.SetOutline (frag.clip);
					// Enables clipping on the View
					clippedView.ClipToOutline = true;

					Log.Debug (TAG, string.Format ("View was clipped"));
					(bt as Button).SetText (Resource.String.unclip_button);
				}
			}
		}

		private class TextListener : Java.Lang.Object,View.IOnClickListener
		{
			ClippingBasicFragment frag;
			public TextListener(ClippingBasicFragment f)
			{
				frag = f;
			}

			public void OnClick(View bt)
			{
				frag.click_count++;
				frag.ChangeText ();
			}
		}

		// When the text is clicked, a new string is shown.
		private void ChangeText() 
		{
			string newText = sample_texts[click_count % sample_texts.Length];

			text_view.SetText (newText.ToCharArray(), 0, newText.Length);
			Log.Debug(TAG,string.Format("Text was changed."));
		}
	}
}

