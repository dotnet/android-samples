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
	[Activity (Label = "@string/fragment_hide_show_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentHideShowSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_hide_show_support);
	
	        // The content view embeds two fragments; now retrieve them and attach
	        // their "hide" button.
	        var fm = SupportFragmentManager;
	        AddShowHideListener(Resource.Id.frag1hide, fm.FindFragmentById(Resource.Id.fragment1));
	        AddShowHideListener(Resource.Id.frag2hide, fm.FindFragmentById(Resource.Id.fragment2));
		}
		
		void AddShowHideListener(int buttonId, Fragment fragment) 
		{
	        var button = FindViewById<Button>(buttonId);
			button.Click += (sender, e) => {
				var ft = SupportFragmentManager.BeginTransaction();
				ft.SetCustomAnimations(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
				if(fragment.IsHidden) {
					ft.Show(fragment);
					button.Text = "Hide";
				} else {
					ft.Hide(fragment);
					button.Text = "Show";
				}
				ft.Commit();
			};
	    }
		
		protected class FirstFragment : Fragment 
		{
			TextView textView;
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				var v = inflater.Inflate(Resource.Layout.labeled_text_edit, container, false);
				var tv = v.FindViewById<TextView>(Resource.Id.msg);
				tv.Text = "The fragment saves and restores this text.";
				
				textView = v.FindViewById<TextView>(Resource.Id.saved);
				if(savedInstanceState != null)
				{
					textView.Text = savedInstanceState.GetString("text");	
				}
				return v;
			}
			
			public override void OnSaveInstanceState (Bundle outState)
			{
				base.OnSaveInstanceState (outState);
				
				outState.PutString("text", textView.Text);
			}
		}
		
		protected class SecondFragment : Fragment
		{
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				var v = inflater.Inflate(Resource.Layout.labeled_text_edit, container, false);
				var tv = v.FindViewById<TextView>(Resource.Id.msg);
				tv.Text = "The TextView saves and restores this text.";
				
				v.FindViewById<TextView>(Resource.Id.saved).SaveEnabled = true;
				return v;
			}	
		}
	}
}

