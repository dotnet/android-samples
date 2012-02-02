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
using Android.Util;

namespace Support4
{
	[Activity (Label = "@string/fragment_alert_dialog_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentAlertDialogSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_dialog);
			
			var tv = FindViewById<TextView>(Resource.Id.text);
			tv.Text = "Example of displaying an alert dialog with a DialogFragment";
			
			var button = FindViewById<Button>(Resource.Id.show);
			button.Click += (sender, e) => {
				ShowDialog();
			};
			
			
		}
		
		void ShowDialog() {
			DialogFragment newFragment = new MyAlertDialogFragment(Resource.String.alert_dialog_two_buttons_title);
	        newFragment.Show(SupportFragmentManager, "dialog");
	    }
	
	    public void DoPositiveClick() 
		{
	        // Do stuff here.
	        Log.Info("FragmentAlertDialog", "Positive click!");
	    }
	
	    public void DoNegativeClick() 
		{
	        // Do stuff here.
	        Log.Info("FragmentAlertDialog", "Negative click!");
	    }
		
		public class MyAlertDialogFragment : DialogFragment 
		{	
    	    public MyAlertDialogFragment (int title) 
			{
	            var args = new Bundle();
	            args.PutInt("title", title);
	            Arguments = args;
	        }
	
	        public override Dialog OnCreateDialog (Bundle savedInstanceState)
			{
				
				var title = Arguments.GetInt("title");
				
				return new AlertDialog.Builder(Activity)
					.SetIcon(Resource.Drawable.alert_dialog_icon)
					.SetTitle(title)
					.SetPositiveButton(Resource.String.alert_dialog_ok, (sender, e) => {
						((FragmentAlertDialogSupport) Activity).DoPositiveClick();	
					})
					.SetNegativeButton(Resource.String.alert_dialog_cancel, (sender, e) => {
						((FragmentAlertDialogSupport) Activity).DoNegativeClick();
					}).Create();
			}
	    }
	}
}

