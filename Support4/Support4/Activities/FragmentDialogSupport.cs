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
	[Activity (Label = "@string/fragment_dialog_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentDialogSupport : FragmentActivity
	{
		int stackLevel;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.fragment_dialog);
	
	        var tv = FindViewById<TextView>(Resource.Id.text);
	        tv.Text = @"Example of displaying dialogs with a DialogFragment.  "
	                + "Press the show button below to see the first dialog; pressing "
	                + "successive show buttons will display other dialog styles as a "
	                + "stack, with dismissing or back going to the previous dialog.";
	
	        // Watch for button clicks.
	        Button button = FindViewById<Button>(Resource.Id.show);
	        button.Click += (sender, e) => {;
	        	ShowDialog();
			};
	
	        if (savedInstanceState != null) {
	            stackLevel = savedInstanceState.GetInt("level");
	        }
		}
		
		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
				
	        outState.PutInt("level", stackLevel);
		}
		
		void ShowDialog() 
		{
	        stackLevel++;
	
	        // DialogFragment.show() will take care of adding the fragment
	        // in a transaction.  We also want to remove any currently showing
	        // dialog, so make our own transaction and take care of that here.
	        var ft = SupportFragmentManager.BeginTransaction();
	        var prev = SupportFragmentManager.FindFragmentByTag("dialog");
	        if (prev != null) {
	            ft.Remove(prev);
	        }
	        ft.AddToBackStack(null);
	
	        // Create and show the dialog.
	        var newFragment = new MyDialogFragment(stackLevel);
	        newFragment.Show(ft, "dialog");
    	}
		
		static string GetNameForNum(int num) {
	        switch ((num-1)%6) {
	            case 1: return "STYLE_NO_TITLE";
	            case 2: return "STYLE_NO_FRAME";
	            case 3: return "STYLE_NO_INPUT (this window can't receive input, so "
	                    + "you will need to press the bottom show button)";
	            case 4: return "STYLE_NORMAL with dark fullscreen theme";
	            case 5: return "STYLE_NORMAL with light theme";
	            case 6: return "STYLE_NO_TITLE with light theme";
	            case 7: return "STYLE_NO_FRAME with light theme";
	            case 8: return "STYLE_NORMAL with light fullscreen theme";
	        }
	        return "STYLE_NORMAL";
    	}
		
		public class MyDialogFragment : DialogFragment
		{
			int num;
			
			public MyDialogFragment()
			{
				
			}
			
			/**
	         * Create a new instance of MyDialogFragment, providing "num"
	         * as an argument.
	         */
			public MyDialogFragment(int num)
			{
				var bundle = new Bundle();
				bundle.PutInt("num", num);
            	Arguments = bundle;
			}
			
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
				
				num = Arguments.GetInt("num");
	
	            // Pick a style based on the num.
	            int style = DialogFragment.StyleNormal, theme = 0;
	            switch ((num-1)%6) {
	                case 1: style = DialogFragment.StyleNoTitle; break;
	                case 2: style = DialogFragment.StyleNoFrame; break;
	                case 3: style = DialogFragment.StyleNoInput; break;
	                case 4: style = DialogFragment.StyleNormal; break;
	                case 5: style = DialogFragment.StyleNoTitle; break;
	                case 6: style = DialogFragment.StyleNoFrame; break;
	                case 7: style = DialogFragment.StyleNormal; break;
	            }
	            switch ((num-1)%6) {
	                case 2: theme = Android.Resource.Style.ThemePanel; break;
	                case 4: theme = Android.Resource.Style.Theme; break;
	                case 5: theme = Android.Resource.Style.ThemeLight; break;
	                case 6: theme = Android.Resource.Style.ThemeLightPanel; break;
	                case 7: theme = Android.Resource.Style.ThemeLight; break;
	            }
	            SetStyle(style, theme);
			}
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				var v = inflater.Inflate(Resource.Layout.fragment_dialog, container, false);
				var tv = v.FindViewById<TextView>(Resource.Id.text);
				tv.Text = "Dialog #" + num + ": using style " + GetNameForNum(num);
				
				var button = v.FindViewById<Button>(Resource.Id.show);
				button.Click += (sender, e) => {
					((FragmentDialogSupport)Activity).ShowDialog();
				};
	            return v;
			}
		}
	}
}

