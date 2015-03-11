
using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Support.V4.App;
using CommonSampleLibrary;

namespace DrawableTinting
{
	public class DrawableTintingFragment : Android.Support.V4.App.Fragment
	{
		public const string TAG = "DrawableTintingFragment";

		// Image applied to programatically
		private ImageView image;

		// Seekbar for alpha component of tinting
		private SeekBar alphaBar;

		// Seekbar for red component of tinting
		private SeekBar redBar;

		// Seekbar for green component of tinting
		private SeekBar greenBar;

		// Seekbar for blue component of tinting
		private SeekBar blueBar;

		// Text label for alpha
		private TextView alphaText;

		// Text label for red
		private TextView redText;

		// Text label for green
		private TextView greenText;

		// Text label for blue
		private TextView blueText;

		// Selector for blend type for color tinting
		private Spinner blendSpinner;

		// Computed color for tinting of drawable
		private int hintColor;

		//Selected color tinting mode
		private PorterDuff.Mode mode;

		private const string STATE_BLEND = "DRAWABLETINTING_BLEND";
		private const string STATE_ALPHA = "DRAWABLETINTING_ALPHA";
		private const string STATE_RED = "DRAWABLETINTING_RED";
		private const string STATE_GREEN = "DRAWABLETINTING_GREEN";
		private const string STATE_BLUE = "DRAWABLETINTING_BLUE";


		// All available tinting modes
		private PorterDuff.Mode[] MODES = new PorterDuff.Mode[]{
			PorterDuff.Mode.Add,
			PorterDuff.Mode.Clear,
			PorterDuff.Mode.Darken,
			PorterDuff.Mode.Dst,
			PorterDuff.Mode.DstAtop,
			PorterDuff.Mode.DstIn,
			PorterDuff.Mode.DstOut,
			PorterDuff.Mode.DstOver,
			PorterDuff.Mode.Lighten,
			PorterDuff.Mode.Multiply,
			PorterDuff.Mode.Overlay,
			PorterDuff.Mode.Screen,
			PorterDuff.Mode.Src,
			PorterDuff.Mode.SrcAtop,
			PorterDuff.Mode.SrcIn,
			PorterDuff.Mode.SrcOut,
			PorterDuff.Mode.SrcOver,
			PorterDuff.Mode.Xor
		};

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			HasOptionsMenu = true;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate (Resource.Layout.tinting_fragment, null);

			// Set a drawable as the image to display
			image = (ImageView)v.FindViewById (Resource.Id.image);
			image.SetImageResource (Resource.Drawable.btn_default_normal_holo);

			// Get text labels and seekbars for the four color components
			alphaBar = (SeekBar)v.FindViewById (Resource.Id.alphaSeek);
			alphaText = (TextView)v.FindViewById (Resource.Id.alphaText);
			greenBar = (SeekBar)v.FindViewById (Resource.Id.greenSeek);
			greenText = (TextView)v.FindViewById (Resource.Id.greenText);
			redBar = (SeekBar)v.FindViewById (Resource.Id.redSeek);
			redText = (TextView)v.FindViewById (Resource.Id.redText);
			blueBar = (SeekBar)v.FindViewById (Resource.Id.blueSeek);
			blueText = (TextView)v.FindViewById (Resource.Id.blueText);

			// Set a listener to update tinted image when selections have changed
			alphaBar.SetOnSeekBarChangeListener (new MySeekBarChangeListener (this));
			greenBar.SetOnSeekBarChangeListener (new MySeekBarChangeListener (this));
			redBar.SetOnSeekBarChangeListener (new MySeekBarChangeListener (this));
			blueBar.SetOnSeekBarChangeListener (new MySeekBarChangeListener (this));


			// Set up the spinner for blend mode selection from a string array resource
			blendSpinner = (Spinner)v.FindViewById (Resource.Id.blendSpinner);
			ISpinnerAdapter sa = ArrayAdapter.CreateFromResource (Activity, 
				Resource.Array.blend_modes, Android.Resource.Layout.SimpleSpinnerDropDownItem);
			blendSpinner.Adapter = sa;
			// Set a listener to update the tinted image when a blend mode is selected
			blendSpinner.OnItemSelectedListener = new MyBlendListener (this);
			// Select the first item
			blendSpinner.SetSelection (0);
			mode = MODES [0];

			if (savedInstanceState != null) {
				// Resore the previous state if this fragment has been restored
				blendSpinner.SetSelection (savedInstanceState.GetInt (STATE_BLEND));
				alphaBar.Progress = savedInstanceState.GetInt (STATE_ALPHA);
				redBar.Progress = savedInstanceState.GetInt (STATE_RED);
				greenBar.Progress = savedInstanceState.GetInt (STATE_GREEN);
				blueBar.Progress = savedInstanceState.GetInt (STATE_BLUE);
			}

			// Apply the default blend mode and color
			UpdateTint (GetColor (), GetTintMode ());

			return v;

		}
		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			Log.Debug (TAG, "state saved.");
			outState.PutInt (STATE_BLEND, blendSpinner.SelectedItemPosition);
			outState.PutInt (STATE_ALPHA, alphaBar.Progress);
			outState.PutInt (STATE_RED, redBar.Progress);
			outState.PutInt (STATE_GREEN, greenBar.Progress);
			outState.PutInt (STATE_BLUE, blueBar.Progress);
		}

		// Computes the Vp;pr value from selections on ARGB sliders
		public int GetColor()
		{
			int alpha = alphaBar.Progress;
			int red = redBar.Progress;
			int green = greenBar.Progress;
			int blue = blueBar.Progress;

			return Color.Argb (alpha, red, green, blue);
		}

		// Returns the selected mode option
		public PorterDuff.Mode GetTintMode()
		{
			return MODES [blendSpinner.SelectedItemPosition];
		}

		// Update the tint of the image based on ARGB sliders and the selected mode option
		public void UpdateTint(int color, PorterDuff.Mode mode)
		{
			// Set the color hint of the image
			hintColor = color;

			// Set the color tint mode based on the selection of the Spinner
			this.mode = mode;

			// Log selection
			Log.Debug (TAG, string.Format("Updating tint with color [ARGB: {0},{1},{2},{3} and mode [{4}]",
				Color.GetAlphaComponent (color), Color.GetRedComponent (color), Color.GetGreenComponent (color), Color.GetBlueComponent (color),mode.ToString()));

			// Apply the color tint for the selected tint mode
			image.SetColorFilter (new Color(hintColor), this.mode);


			// Update the text for each label with the value for each channel
			alphaText.SetText("Alpha: " + alphaBar.Progress.ToString(),TextView.BufferType.Normal);
			redText.SetText ("Red: " + redBar.Progress.ToString(),TextView.BufferType.Normal);
			greenText.SetText ("Green: " + greenBar.Progress.ToString(),TextView.BufferType.Normal);
			blueText.SetText ("Blue: " + blueBar.Progress.ToString(),TextView.BufferType.Normal);

		}

		// Listener that updates the tint when a blend item is selected
		private class MyBlendListener : Java.Lang.Object,AdapterView.IOnItemSelectedListener
		{
			public DrawableTintingFragment dtf;
			public MyBlendListener(DrawableTintingFragment d)
			{
				dtf = d;
			}

			public void OnItemSelected(AdapterView adapterView, View view, int i, long l)
			{
				dtf.UpdateTint (dtf.GetColor (), dtf.GetTintMode ());
			
			}
			public void OnNothingSelected(AdapterView adapterView)
			{
			}

		}

		// Listener that updates the tinted color when the progress bar has changed
		private class MySeekBarChangeListener : Java.Lang.Object,SeekBar.IOnSeekBarChangeListener
		{
			public DrawableTintingFragment dtf;
			public MySeekBarChangeListener(DrawableTintingFragment d)
			{
				dtf = d;
			}
			public void OnProgressChanged(SeekBar seekBar, int i, bool b)
			{
				dtf.UpdateTint(dtf.GetColor(),dtf.GetTintMode());
			}

			public void OnStartTrackingTouch(SeekBar seekBar)
			{
			}

			public void OnStopTrackingTouch(SeekBar seekBar)
			{
			}
		}
	}
}

