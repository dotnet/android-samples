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
	[Activity (Label = "@string/fragment_arguments_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentArgumentsSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_arguments_support);
			
	        if (bundle == null) {
	            // First-time init; create fragment to embed in activity.
	            FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
	            var newFragment = new MyFragment("From Arguments");
				ft.Add(Resource.Id.created, newFragment);
				ft.Commit();
	        }
		}
		
		public class MyFragment : Fragment
		{
			string mLabel;
			
			public MyFragment()
			{
				
			}
			
			/**
	         * Create a new instance of MyFragment that will be initialized
	         * with the given arguments.
	         */
			public MyFragment(string label)
			{
				var bundle = new Bundle();
				bundle.PutString("label", label);
            	Arguments = bundle;
			}
			
			/**
	         * Parse attributes during inflation from a view hierarchy into the
	         * arguments we handle.
	         */
			public override void OnInflate (Activity activity, Android.Util.IAttributeSet attrs, Bundle savedInstanceState)
			{
				base.OnInflate (activity, attrs, savedInstanceState);
				
				var a = activity.ObtainStyledAttributes(attrs,Resource.Styleable.FragmentArguments);
		        mLabel = a.GetText(Resource.Styleable.FragmentArguments_android_label);
		        a.Recycle();
			}
			
			/**
	         * During creation, if arguments have been supplied to the fragment
	         * then parse those out.
	         */
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				
				var args = Arguments;
	            if (args != null) {
	                string label = args.GetString("label");
	                if (label != null) {
	                    mLabel = label;
	                }
	            }
			}
			
			/**
	         * Create the view for this fragment, using the arguments given to it.
	         */
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView (inflater, container, savedInstanceState);
				
				var v = inflater.Inflate(Resource.Layout.hello_world, container, false);
            	var tv = v.FindViewById<TextView>(Resource.Id.text);
            	tv.Text = mLabel != null ? mLabel : "(no label)";
	            tv.SetBackgroundDrawable(Resources.GetDrawable(Android.Resource.Drawable.GalleryThumb));
	            return v;
			}
		}
	}
}

