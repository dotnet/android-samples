using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Uri = Android.Net.Uri;
using MonoIO.Utilities;

namespace MonoIO.UI
{
	public class BaseActivity : FragmentActivity
	{
		private ActivityHelper helper;

		protected BaseActivity () : base ()
		{
			helper = ActivityHelperFactory.Create (this);
		}

		protected override void OnPostCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);

			helper.OnPostCreate (savedInstanceState);
		}

		public override bool OnKeyLongPress (Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			return helper.OnKeyLongPress (keyCode, e) ||  base.OnKeyLongPress (keyCode, e);
		}

		public override bool OnKeyDown (Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			return helper.OnKeyDown (keyCode, e) || base.OnKeyDown (keyCode, e);
		}

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			return helper.OnCreateOptionsMenu (menu) || base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			return helper.OnOptionsItemSelected (item) || base.OnOptionsItemSelected (item);
		}

		public ActivityHelper ActivityHelper { get { return helper; } }
		
		/**
	     * Takes a given intent and either starts a new activity to handle it (the default behavior),
	     * or creates/updates a fragment (in the case of a multi-pane activity) that can handle the
	     * intent.
	     *
	     * Must be called from the main (UI) thread.
	     */
	    public virtual void OpenActivityOrFragment(Intent intent) {
	        // Default implementation simply calls startActivity
	        StartActivity(intent);
	    }
		
		/**
	     * Converts an intent into a {@link Bundle} suitable for use as fragment arguments.
	     */
	    public static Bundle IntentToFragmentArguments(Intent intent) {
	        var arguments = new Bundle();
	        if (intent == null) {
	            return arguments;
	        }
	
	        var data = intent.Data;
	        if (data != null) {
	            arguments.PutParcelable("_uri", data);
	        }
	
	        var extras = intent.Extras;
	        if (extras != null) {
	            arguments.PutAll(intent.Extras);
	        }
	
	        return arguments;
	    }
		
		
		public static Intent FragmentArgumentsToIntent (Bundle arguments)
		{
			Intent intent = new Intent();
	        if (arguments == null) {
	            return intent;
	        }
	
	        Uri data = (Uri) arguments.GetParcelable("_uri");
	        if (data != null) {
	            intent.SetData(data);
	        }
	
	        intent.PutExtras(arguments);
	        intent.RemoveExtra("_uri");
	        return intent;
		}
	}
}
