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
	[Activity (Label = "@string/fragment_dialog_or_activity_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentDialogOrActivitySupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		
			SetContentView(Resource.Layout.fragment_dialog_or_activity);
			
			if(bundle == null)
			{
				// First-time init; create fragment to embed in activity.
				var ft = SupportFragmentManager.BeginTransaction();
				var newFragment = new MyDialogFragment();
				ft.Add(Resource.Id.embedded, newFragment);
				ft.Commit();
			}
			
			var button = FindViewById<Button>(Resource.Id.show_dialog);
			button.Click += (sender, e) => {
				ShowDialog();
			};
		}
		
		void ShowDialog() {
	        // Create the fragment and show it as a dialog.
	        DialogFragment newFragment = new MyDialogFragment();
	        newFragment.Show(SupportFragmentManager, "dialog");
	    }
		
		public class MyDialogFragment : DialogFragment 
		{	
	
			public MyDialogFragment()
			{
				
			}
			
	        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle p2)
			{
				var v = inflater.Inflate(Resource.Layout.hello_world, container, false);
	            var tv = v.FindViewById<TextView>(Resource.Id.text);
	            tv.Text = "This is an instance of MyDialogFragment";
	            return v;
			}
	    }
	}
}

