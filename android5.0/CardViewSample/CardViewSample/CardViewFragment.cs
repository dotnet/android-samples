
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace CardViewSample
{
	public class CardViewFragment : Fragment
	{
		private string TAG = Java.Lang.Class.FromType (typeof(CardViewFragment)).SimpleName;

		// The CardView widget
		CardView cardView;

		// SeekBar that changes the corner radius of the CardView
		SeekBar radiusSeekBar;

		// SeekBar that changes the elevation of the CardView
		SeekBar elevationSeekBar;

		public static CardViewFragment NewInstance()
		{
			CardViewFragment fragment = new CardViewFragment ();
			fragment.RetainInstance = true;
			return fragment;
		}

		public CardViewFragment()
		{
		}
			
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_card_view, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			cardView = (CardView)view.FindViewById<CardView> (Resource.Id.cardview);
			radiusSeekBar = (SeekBar)view.FindViewById (Resource.Id.cardview_radius_seekbar);
			radiusSeekBar.ProgressChanged += delegate {
				Log.Debug (TAG, string.Format ("SeekBar Radius progress : {0}", radiusSeekBar.Progress));
				cardView.Radius = radiusSeekBar.Progress;
			};

			elevationSeekBar = (SeekBar)view.FindViewById (Resource.Id.cardview_elevation_seekbar);
			elevationSeekBar.ProgressChanged+= delegate {
				Log.Debug(TAG,string.Format("SeekBar Elevation progress : {0}",elevationSeekBar.Progress));
				cardView.Elevation = elevationSeekBar.Progress;
			};
		}
	}
}

