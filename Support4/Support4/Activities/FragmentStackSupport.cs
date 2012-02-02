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
	[Activity (Label = "@string/fragment_stack_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentStackSupport : FragmentActivity
	{
		int stackLevel = 1;
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			SetContentView(Resource.Layout.fragment_stack);

	        // Watch for button clicks.
	        var button = FindViewById<Button>(Resource.Id.new_fragment);
	        button.Click += (sender, e) => {
				AddFragmentToStack();
			};
			
			if(savedInstanceState == null)
			{
				var newFragment = new CountingFragment(stackLevel);
				var ft = SupportFragmentManager.BeginTransaction();
				ft.Add(Resource.Id.simple_fragment, newFragment).Commit();
			}
			else
			{
				stackLevel = savedInstanceState.GetInt("level");	
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
	        ft.Replace(Resource.Id.simple_fragment, newFragment);
	        ft.SetTransition(FragmentTransaction.TransitFragmentOpen);
	        ft.AddToBackStack(null);
	        ft.Commit();
		}
		
		public class CountingFragment : Fragment
		{
			int _num;
			
			public CountingFragment()
			{
				
			}
			
			public CountingFragment(int num)
			{
				// Supply num input as an argument.
	            var args = new Bundle();
	            args.PutInt("num", num);
	            Arguments = args;
			}
			
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				
				_num = Arguments != null ? Arguments.GetInt("num") : 1;
			}
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				var v = inflater.Inflate(Resource.Layout.hello_world, container, false);
	            var tv = v.FindViewById<TextView>(Resource.Id.text);
	            tv.Text = "Fragment #" + _num;
	            tv.SetBackgroundDrawable(Resources.GetDrawable(Android.Resource.Drawable.GalleryThumb));
	            return v;
			}
		}
	}
}

