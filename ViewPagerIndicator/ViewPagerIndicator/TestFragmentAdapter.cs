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
using Android.Support.V4.App;

using Fragment        = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace ViewPagerIndicator
{
	public class TestFragmentAdapter : FragmentPagerAdapter
	{
		public static string[] CONTENT = new string[] { "This", "Is", "A", "Test", };
		int mCount;
		
		public TestFragmentAdapter (FragmentManager fm) : base (fm)
		{
			mCount = CONTENT.Count ();
		}
		
		public override Fragment GetItem (int position)
		{
			return new TestFragment (CONTENT [position % CONTENT.Count ()]);
		}
		
		public override int Count {
			get {
				return mCount;	
			}	
		}
		
		public void SetCount (int count)
		{
			Console.WriteLine ("Setting count to " + count);
			if (count > 0 && count <= 10) {
				mCount = count;
				NotifyDataSetChanged ();
			}
		}
	}
}

