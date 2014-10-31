
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Animation;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using CommonSampleLibrary;

namespace Interpolator
{
	public class InterpolatorFragment : Fragment
	{
		// View that is animated
		private View view;

		// Spinner for selection of interpolator
		private Spinner interpolatorSpinner;

		// SeekBar for selection of duration of animation
		private SeekBar durationSeekbar;

		// TextView that shows animation selected in SeekBar
		private TextView durationLabel;

		// Interpolators used for animation
		private IInterpolator[] interpolators;

		// Path for in (shrinking) animation, from 100% scale to 20%
		private Path pathIn;

		// Path for out (growing) animation, from 20% scale to 100%
		private Path pathOut;

		// Set to true if View is animated out (shrunk)
		private bool isOut = false;

		// Default duration of animation
		private const int INITIAL_DURATION_MS = 750;

		// Tag used for logging
		public const string TAG = "InterpolatorFragment";


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Inflate the interpolator_fragment layout
			var v = inflater.Inflate (Resource.Layout.interpolator_fragment, container, false);

			// Set up the "animate" button
			// When it is clicked, the view is animatied with the options selected
			var button = v.FindViewById<Button> (Resource.Id.animateButton);
			button.Click += delegate {
				// Interpolator selected in the spinner
				var interpolator = interpolators [interpolatorSpinner.SelectedItemPosition];
				// Duration selected in SeekBar
				long duration = (long)durationSeekbar.Progress;
				// Animation path is based on whether animating in or out
				var path = isOut ? pathIn : pathOut;

				// Log animation details
				Log.Info (TAG, string.Format ("Starting animation: [{0} ms, {1}, {2}",
					duration, interpolatorSpinner.SelectedItem.ToString (),
					((isOut) ? "Out (growing)" : "In (shrinking)")));

				// Start the animation with the selected options
				StartAnimation(interpolator,duration,path);

				// Toggle direction of the animation
				isOut = !isOut;

			};
				
			durationLabel = v.FindViewById<TextView> (Resource.Id.durationLabel);

			// Initialize the Interpolators programmatically by loading them from their XML definitions
			// provided by the framework.
			interpolators = new IInterpolator[]
			{
				AnimationUtils.LoadInterpolator(Activity,Android.Resource.Interpolator.Linear),
				AnimationUtils.LoadInterpolator(Activity,Android.Resource.Interpolator.FastOutLinearIn),
				AnimationUtils.LoadInterpolator(Activity,Android.Resource.Interpolator.FastOutSlowIn),
				AnimationUtils.LoadInterpolator(Activity,Android.Resource.Interpolator.LinearOutSlowIn)
			};

			// Load names of interpolators
			var interpolatorNames = Resources.GetStringArray (Resource.Array.interpolator_names);

			// Set up the Spinner with the names of interpolators
			interpolatorSpinner = v.FindViewById<Spinner> (Resource.Id.interpolatorSpinner);
			var spinnerAdapter = new ArrayAdapter<string> (Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, interpolatorNames);
			interpolatorSpinner.Adapter = spinnerAdapter;

			// Set up SeekBar that defines the duration of the animation
			durationSeekbar = v.FindViewById<SeekBar> (Resource.Id.durationSeek);
			durationSeekbar.ProgressChanged+=delegate {
				durationLabel.SetText(Resources.GetString(Resource.String.animation_duration,durationSeekbar.Progress),TextView.BufferType.Normal);
			};

			// Set default duration
			durationSeekbar.Progress = INITIAL_DURATION_MS;

			// Get the view that will be animated
			view = v.FindViewById (Resource.Id.square);

			// Path for the "in" animation
			pathIn = new Path ();
			pathIn.MoveTo (0.2f, 0.2f);
			pathIn.LineTo (1f, 1f);

			// Path for the "out" animation
			pathOut = new Path ();
			pathOut.MoveTo (1f, 1f);
			pathOut.LineTo (0.2f, 0.2f);
			return v;

		}

		public ObjectAnimator StartAnimation(IInterpolator interpolator, long duration, Path path)
		{
			// This ObjectAnimator uses the path to change the x and y scale of the view object
			ObjectAnimator animator = ObjectAnimator.OfFloat(view,"ScaleX","ScaleY",path);

			// Set the interpolator and duration for this animation
			animator.SetDuration (duration);
			animator.SetInterpolator (interpolator);

			animator.Start ();

			return animator;
		}

		// Getters
		public IInterpolator[] Interpolators
		{
			get{ return interpolators; }
		}

		public Path PathIn
		{
			get{ return pathIn; }
		}

		public Path PathOut
		{
			get { return pathOut; }
		}
	}
}

