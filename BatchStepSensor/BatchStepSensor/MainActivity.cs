using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CommonSampleLibrary;
using BatchStepSensor.CardStream;

namespace BatchStepSensor
{
	[Activity (Label = "BatchStepSensor", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : SampleActivityBase, BatchStepSensor.CardStream.CardStream
	{
		new public const string TAG = "MainActivity";
		public const string FRAGTAG = "BatchStepSensorFragment";

		CardStreamFragment mCardStreamFragment;

		StreamRetentionFragment mRetentionFragment;
		const string RETENTION_TAG = "retention";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);

			var fm = SupportFragmentManager;
			var fragment = (BatchStepSensorFragment)fm.FindFragmentByTag (FRAGTAG);

			if (fragment == null) {
				var transaction = fm.BeginTransaction ();
				fragment = new BatchStepSensorFragment ();
				transaction.Add (fragment, FRAGTAG);
				transaction.Commit ();
			}

			// Use fragment as click listener for cards, but must implement correct interface
			if (!(fragment is OnCardClickListener)) {
				throw new InvalidCastException ("BatchStepSensorFragment must implement OnCardClickListener interface.");
			}
			var clickListener = (OnCardClickListener)fm.FindFragmentByTag (FRAGTAG);
			mRetentionFragment = (StreamRetentionFragment)fm.FindFragmentByTag (RETENTION_TAG);
			if (mRetentionFragment == null) {
				mRetentionFragment = new StreamRetentionFragment ();
				fm.BeginTransaction ().Add (mRetentionFragment, RETENTION_TAG).Commit ();
			} else {
				// If the retention fragment already existed, we need to pull a state.
				// Pull State out.
				CardStreamState state = mRetentionFragment.CardStream;

				// Dump it in CardStreamFragment;
				mCardStreamFragment = (CardStreamFragment)fm.FindFragmentById (Resource.Id.fragment_cardstream);
				mCardStreamFragment.RestoreState (state, clickListener);
			}
		}

		public CardStreamFragment CardStream
		{
			get
			{
				if (mCardStreamFragment == null) {
					mCardStreamFragment = (CardStreamFragment)SupportFragmentManager.FindFragmentById (Resource.Id.fragment_cardstream);
				}
				return mCardStreamFragment;
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			CardStreamState state = CardStream.DumpState ();
			mRetentionFragment.CardStream = state;
		}

		public CardStreamFragment GetCardStream ()
		{
			return CardStream;
		}
	}
}


