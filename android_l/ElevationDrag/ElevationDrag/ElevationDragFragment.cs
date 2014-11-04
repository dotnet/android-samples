
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
using Android.Graphics;

using CommonSampleLibrary;

namespace ElevationDrag
{
	public class ElevationDragFragment : Android.Support.V4.App.Fragment, AdapterView.IOnItemSelectedListener
	{
		public const string TAG = "ElevationFragment";

		/* How much to translate each time the Z+ and Z- buttons are clicked. */
		private static int ELEVATION_STEP = 40;

		/* Different outlines: */
		internal Outline mOutline;
		internal Outline mOutline2;
		CircleOutlineProvider circleProvider;
		RectOutlineProvider rectProvider;

		View floatingShape;

		/* The current elevation of the floating view. */
		private float mElevation = 0;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate (Resource.Layout.ztranslation, container, false);


			//Find view to apply Z-translation to
			floatingShape = rootView.FindViewById (Resource.Id.circle);

			//Create the outlines
			mOutline = new Outline ();
			mOutline2 = new Outline ();

			circleProvider = new CircleOutlineProvider ();
			circleProvider.GetOutline (floatingShape, mOutline);

			rectProvider = new RectOutlineProvider ();
			rectProvider.GetOutline (floatingShape, mOutline2);
			/*
			//Define the view's shape
			floatingShape.OutlineProvider = circleProvider;
			//Clip view to outline
			floatingShape.ClipToOutline = true;
			*/
			var dragLayout = rootView.FindViewById<DragFrameLayout> (Resource.Id.main_layout);

			dragLayout.mDragFrameLayoutController = new DragFrameLayoutController ((bool captured) => {
				floatingShape.Animate ()
					.TranslationZ (captured ? 50 : 0)
					.SetDuration (100);
				Log.Debug (TAG, captured ? "drag" : "drop");
			});

			dragLayout.AddDragView (floatingShape);

			rootView.FindViewById (Resource.Id.raise_bt).Click += delegate {
				mElevation += ELEVATION_STEP;
				Log.Debug (TAG, string.Format ("Elevation: {0:0.#}", mElevation));
				floatingShape.Elevation = mElevation;
			};

			rootView.FindViewById (Resource.Id.lower_bt).Click += delegate {
				mElevation = Math.Max (mElevation - ELEVATION_STEP, 0);
				Log.Debug (TAG, string.Format ("Elevation: {0:0.#}", mElevation));
				floatingShape.Elevation = mElevation;
			};

			/* Create a spinner with options to change the shape of the object. */
			var spinner = rootView.FindViewById<Spinner> (Resource.Id.shapes_spinner);
			spinner.Adapter = new ArrayAdapter<string> (
				this.Activity,
				Android.Resource.Layout.SimpleSpinnerDropDownItem,
				this.Resources.GetStringArray (Resource.Array.shapes));

			spinner.OnItemSelectedListener = this;
			return rootView;
		}

		public void OnItemSelected (AdapterView parent, View view, int position, long id)
		{
			/* Set the corresponding Outline to the shape. */
			switch (position) {
			case 0:
				floatingShape.OutlineProvider = circleProvider;
				floatingShape.ClipToOutline = true;
				break;
			case 1:
				floatingShape.OutlineProvider = rectProvider;
				floatingShape.Invalidate ();
				floatingShape.ClipToOutline = true;
				break;
			default:
				floatingShape.OutlineProvider = circleProvider;
				/* Don't clip the view to the outline in the last case. */
				floatingShape.ClipToOutline = false;
				break;
			}
		}

		public void OnNothingSelected (AdapterView parent)
		{
			floatingShape.OutlineProvider = circleProvider;
		}

		private class RectOutlineProvider : ViewOutlineProvider
		{
			public override void GetOutline (View view, Outline outline)
			{
				int shapeSize = view.Resources.GetDimensionPixelSize (Resource.Dimension.shape_size);
				outline.SetRoundRect (0, 0, shapeSize, shapeSize, shapeSize / 10);
			}
		}

		private class CircleOutlineProvider : ViewOutlineProvider
		{
			public override void GetOutline (View view, Outline outline)
			{
				int shapeSize = view.Resources.GetDimensionPixelSize (Resource.Dimension.shape_size);
				outline.SetRoundRect (0, 0, shapeSize, shapeSize, shapeSize / 2);

			}
		}


	}

}

