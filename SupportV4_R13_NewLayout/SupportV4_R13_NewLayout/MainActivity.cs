using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Support.V4;

namespace SupportV4R13
{
	[Activity (Label = "SupportV4R13NewLayout", MainLauncher = true)]
	public class MainActivity : ListActivity
	{
		string[] items;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			items = new string[] { "DrawerLayout","SlidingPaneLayout"};
			ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);

		}
		protected override void OnListItemClick(ListView l, View v, int position, long id){
			switch (position) 
			{
			case 0:
				StartActivity (typeof(DrawerLayoutActivity));
				break;
			case 1:
				StartActivity (typeof(SlidingPaneLayoutActivity));
				break;
			}
		}
	}
}


