using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Ads;
using Android;

namespace AdMobExample
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		protected AdView mAdView;
		protected InterstitialAd mInterstitialAd;
		protected Button mLoadInterstitialButton;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			mAdView = FindViewById<AdView> (Resource.Id.adView);
			var adRequest = new AdRequest.Builder ().Build ();
			mAdView.LoadAd (adRequest);

			mInterstitialAd = new InterstitialAd (this);
			mInterstitialAd.AdUnitId = GetString (Resource.String.test_interstitial_ad_unit_id);

			mInterstitialAd.AdListener = new AdListener (this);

			mLoadInterstitialButton = FindViewById<Button> (Resource.Id.load_interstitial_button);
			mLoadInterstitialButton.SetOnClickListener (new OnClickListener (this));
		}

		protected void RequestNewInterstitial ()
		{
			var adRequest = new AdRequest.Builder ().Build ();
			mInterstitialAd.LoadAd (adRequest);
		}

		protected void BeginSecondActivity ()
		{
			var intent = new Intent (this, typeof(SecondActivity));
			StartActivity (intent);
		}

		protected override void OnPause ()
		{
			if (mAdView != null) {
				mAdView.Pause ();
			}
			base.OnPause ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (mAdView != null) {
				mAdView.Resume ();
			}
			if (!mInterstitialAd.IsLoaded) {
				RequestNewInterstitial ();
			}
		}

		protected override void OnDestroy ()
		{
			if (mAdView != null) {
				mAdView.Destroy ();
			}
			base.OnDestroy ();
		}

		class AdListener : Android.Gms.Ads.AdListener
		{
			MainActivity that;

			public AdListener (MainActivity t)
			{
				that = t;
			}

			public override void OnAdClosed ()
			{
				that.RequestNewInterstitial ();
				that.BeginSecondActivity ();
			}
		}

		class OnClickListener : Java.Lang.Object, View.IOnClickListener
		{
			MainActivity that;

			public OnClickListener (MainActivity t)
			{
				that = t;
			}

			public void OnClick (View v)
			{
				if (that.mInterstitialAd.IsLoaded) {
					that.mInterstitialAd.Show ();
				} else {
					that.BeginSecondActivity ();
				}
			}
		}
	}
}


