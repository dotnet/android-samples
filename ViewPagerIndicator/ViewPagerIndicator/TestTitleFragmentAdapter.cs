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

using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace ViewPagerIndicator
{
	public class TestTitleFragmentAdapter : TestFragmentAdapter, TitleProvider
	{
		public TestTitleFragmentAdapter (FragmentManager fm) : base(fm)
		{	
		}
		
		public string GetTitle (int position)
		{
			return TestFragmentAdapter.CONTENT [position % CONTENT.Count ()];
		}
	}
}

