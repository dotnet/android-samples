using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Analytics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using System.Linq;
using System.Globalization;

namespace Analytics
{
	[Activity (MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		const string Tag = "MainActivity";

		static IEnumerable<ImageInfo> ImageInfos {
			get {
				yield return new ImageInfo { Image = Resource.Drawable.favorite, Title = Resource.String.pattern1_title };
				yield return new ImageInfo { Image = Resource.Drawable.flash, Title = Resource.String.pattern2_title };
				yield return new ImageInfo { Image = Resource.Drawable.face, Title = Resource.String.pattern3_title };
				yield return new ImageInfo { Image = Resource.Drawable.whitebalance, Title = Resource.String.pattern4_title };
			}
		}

		ImagePagerAdapter mImagePagerAdapter;

		ViewPager mViewPager;

		Tracker mTracker;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			var application = (AnalyticsApplication)Application;
			mTracker = application.DefaultTracker;

			mImagePagerAdapter = new ImagePagerAdapter (this, SupportFragmentManager, ImageInfos.ToArray());

			mViewPager = FindViewById<ViewPager> (Resource.Id.pager);
			mViewPager.Adapter = mImagePagerAdapter;

			mViewPager.AddOnPageChangeListener (new SimpleOnPageChangeListener (this));

			SendScreenImageName ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_share:
				mTracker.Send (new HitBuilders.EventBuilder ().SetCategory ("Action").SetAction ("Share").Build ());
				var name = CurrentImageTitle;
				var text = "I'd love you to hear about " + name;

				var sendIntent = new Intent ();
				sendIntent.SetAction (Intent.ActionSend);
				sendIntent.PutExtra (Intent.ExtraText, text);
				sendIntent.SetType ("text/plain");
				StartActivity (sendIntent);
				break;
			}
			return false;
		}

		string CurrentImageTitle {
			get {
				var position = mViewPager.CurrentItem;
				var info = ImageInfos.ToArray() [position];
				return GetString (info.Title);
			}
		}

		protected void SendScreenImageName ()
		{
			var name = CurrentImageTitle;

			Log.Info (Tag, "Setting screen name: " + name);
			mTracker.SetScreenName ("Image~" + name);
			mTracker.Send (new HitBuilders.ScreenViewBuilder ().Build ());
		}

		public class ImagePagerAdapter : FragmentPagerAdapter
		{
			readonly ImageInfo[] infos;
			readonly Activity that;
			public ImagePagerAdapter (Activity t, Android.Support.V4.App.FragmentManager fm, ImageInfo[] infos) : base (fm)
			{
				this.infos = infos;
				that = t;
			}

			public override int Count {
				get {
					return infos.Length;
				}
			}

			public override Android.Support.V4.App.Fragment GetItem (int position)
			{
				var info = infos [position];
				return ImageFragment.Create (info.Image);
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
			{
				if (position < 0 || position >= infos.Length) {
					return null;
				}
				var locale = CultureInfo.CurrentCulture;
				var info = infos [position];
				return new Java.Lang.String (that.GetString (info.Title).ToUpper (locale));

			}
		}

		class SimpleOnPageChangeListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
		{
			readonly MainActivity that;

			public SimpleOnPageChangeListener (MainActivity t)
			{
				that = t;
			}

			public void OnPageScrollStateChanged (int state)
			{
			}

			public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
			{
			}

			public void OnPageSelected (int position)
			{
				that.SendScreenImageName ();
			}
		}
	}
}


