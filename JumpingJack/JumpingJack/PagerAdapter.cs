using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V13.App;

using Java.Util;

namespace JumpingJack
{
	public class PagerAdapter : FragmentPagerAdapter
	{
		List<Fragment> fragments = null;
		public PagerAdapter (FragmentManager fm) : base(fm)
		{
			fragments = new List<Fragment> ();
		}

		public override Fragment GetItem (int position)
		{
			return fragments [position];
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
}

