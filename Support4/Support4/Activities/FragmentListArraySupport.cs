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

namespace Support4
{
	[Activity (Label = "@string/fragment_list_array_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentListArraySupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Create the list fragment and add it as our sole content.
	        if (SupportFragmentManager.FindFragmentById(Android.Resource.Id.Content) == null) {
	            var list = new ArrayListFragment();
	            SupportFragmentManager.BeginTransaction().Add(Android.Resource.Id.Content, list).Commit();
	        }
		}
		
		public class ArrayListFragment : ListFragment 
		{
			public override void OnActivityCreated (Bundle p0)
			{
				base.OnActivityCreated (p0);
		
				ListAdapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItem1, Shakespeare.TITLES);
	        }
	
	     	public override void OnListItemClick(ListView l, View v, int position, long id) {
	            Console.WriteLine ("Item clicked: " + id);
	        }
    	}
	}
}

