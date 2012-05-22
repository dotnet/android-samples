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
using Android.Support.V4.View;

namespace Support4
{
	[Activity (Label = "@string/fragment_state_pager_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentStatePagerSupport : FragmentActivity
	{
		const int NUM_ITEMS = 10;
	    MyAdapter adapter;
	    ViewPager pager;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_pager);
			
			adapter = new MyAdapter(SupportFragmentManager);
			
			pager = FindViewById<ViewPager>(Resource.Id.pager);
			pager.Adapter = adapter;
			
			var button = FindViewById<Button>(Resource.Id.goto_first);
			button.Click += (sender, e) => {
				pager.CurrentItem = 0;	
			};
			button = FindViewById<Button>(Resource.Id.goto_last);
			button.Click += (sender, e) => {
				pager.CurrentItem = NUM_ITEMS - 1;
			};
		}
		
		protected class MyAdapter : FragmentStatePagerAdapter 
		{
	        public MyAdapter(FragmentManager fm) : base(fm)
			{
			}
	
	       	public override int Count {
				get {
					return NUM_ITEMS;
				}
			}
			
			public override Fragment GetItem (int position)
			{
				return new ArrayListFragment(position);
			}
	
	    }
		
		protected class ArrayListFragment : ListFragment
		{
			int num;
			
			public ArrayListFragment()
			{	
			}
			
			public ArrayListFragment(int num)
			{
				var args = new Bundle();
				args.PutInt("num", num);
				Arguments = args;
			}
			
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
				
				num = Arguments != null ? Arguments.GetInt("num") : 1;
			}
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				var v = inflater.Inflate(Resource.Layout.fragment_pager_list, container, false);
	            var tv = v.FindViewById<TextView>(Resource.Id.text);
	            tv.Text = "Fragment #" + num;
	            return v;
			}
			
			public override void OnActivityCreated (Bundle p0)
			{
				base.OnActivityCreated (p0);
				
				ListAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, Cheeses.cheeseStrings);
			}
			
        	public override void OnListItemClick(ListView l, View v, int position, long id) {
            	Console.WriteLine ("Item clicked: " + id);
        	}
		}
	}
}

