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
	[Activity (Label = "@string/fragment_custom_animation_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentCustomAnimationSupport : FragmentActivity
	{
		int stackLevel = 1;
	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_stack);
			
			var button = FindViewById<Button>(Resource.Id.new_fragment);
			button.Click += (sender, e) => {
				AddFragmentToStack();
			};
			
			if(bundle == null)
			{
				// Do first time initialization -- add initial fragment.
				var newFragment = new CountingFragment(stackLevel);
				var ft = SupportFragmentManager.BeginTransaction();
				ft.Add (Resource.Id.simple_fragment, newFragment).Commit();
			}
			else
			{
				stackLevel = bundle.GetInt("level");
			}
		}
		
		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutInt("level", stackLevel);
		}
		
		void AddFragmentToStack()
		{
			stackLevel++;

	        // Instantiate a new fragment.
	        var newFragment = new CountingFragment(stackLevel);
	
	        // Add the fragment to the activity, pushing this transaction
	        // on to the back stack.
	        var ft = SupportFragmentManager.BeginTransaction();
	        ft.SetCustomAnimations(Resource.Animation.fragment_slide_left_enter,
	                Resource.Animation.fragment_slide_left_exit,
	                Resource.Animation.fragment_slide_right_enter,
	                Resource.Animation.fragment_slide_right_exit);
	        ft.Replace(Resource.Id.simple_fragment, newFragment);
	        ft.AddToBackStack(null);
	        ft.Commit();
		}
		
		public class CountingFragment : Fragment
		{
			int num;
			
			public CountingFragment()
			{
			}
			
			public CountingFragment(int stackLevel)
			{
				var bundle = new Bundle();
				bundle.PutInt("num", stackLevel);
            	Arguments = bundle;
			}
			
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
				
				num = Arguments != null ? Arguments.GetInt("num") : 1;
			}
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle p2)
			{
				var v = inflater.Inflate(Resource.Layout.hello_world, container, false);
	            var tv = v.FindViewById<TextView>(Resource.Id.text);
	            tv.Text = "Fragment #" + num;
	            tv.SetBackgroundDrawable(Resources.GetDrawable(Android.Resource.Drawable.GalleryThumb));
	            return v;
			}
			
		}
	}
}

