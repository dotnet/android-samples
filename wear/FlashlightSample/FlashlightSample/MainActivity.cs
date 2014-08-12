using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V13.App;
using Android.Support.V4.View;


namespace FlashlightSample
{
	[Activity (Label = "FlashlightSample", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{
		private ViewPager view_pager;
		public PartyLightFragment party_fragment;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			view_pager = (FindViewById<ViewPager> (Resource.Id.pager));

			//make the first page the white light fragment
			LightFragmentAdapter adapter = new LightFragmentAdapter (FragmentManager);
			adapter.AddFragment (new WhiteLightFragment ());

			//make the second page the party light fragment
			party_fragment = new PartyLightFragment ();
			adapter.AddFragment (party_fragment);

			view_pager.Adapter = adapter;
			view_pager.SetOnPageChangeListener (new MyOnPageListener (this));

		}
	}

	public class LightFragmentAdapter : FragmentPagerAdapter
	{
		private List<Fragment> fragments;


		public LightFragmentAdapter(FragmentManager fm) : base(fm)
		{
			fragments = new List<Fragment> ();
		}

		public override Fragment GetItem (int position)
		{
			return fragments[position];
		}

		public override int Count {
			get {
				return fragments.Count;
			}
		}

		public void AddFragment(Fragment fragment)
		{
			fragments.Add (fragment);
			NotifyDataSetChanged ();
		}

	}

	public class WhiteLightFragment : Fragment
	{
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.white_light, container, false);
		}
	}

	public class PartyLightFragment : Fragment
	{
		private PartyLightView view;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = (PartyLightView)inflater.Inflate (Resource.Layout.party_light, container, false);
			return view;
		}

		public void StartCycling()
		{
			view.StartCycling ();
		}

		public void StopCycling()
		{
			view.StopCycling ();
		}
	}

	public class MyOnPageListener : Java.Lang.Object,ViewPager.IOnPageChangeListener
	{
		MainActivity a;
		public MyOnPageListener(MainActivity activity)
		{
			a = activity;
		}
		public void OnPageSelected(int position)
		{
			if (position == 1) {
				a.party_fragment.StartCycling ();
			} else {
				a.party_fragment.StopCycling ();
			}
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
		}

		public void OnPageScrollStateChanged(int state)
		{
		}
	}
}


