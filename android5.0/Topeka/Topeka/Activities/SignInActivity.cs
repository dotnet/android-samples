using System;
using Android.App;
using Android.Views;
using Android.Content;
using Topeka.Fragments;
using Topeka.Helpers;

namespace Topeka.Activities
{
	[Activity (WindowSoftInputMode = SoftInput.AdjustPan, MainLauncher = true)]
	public class SignInActivity : Activity
	{
		const string ExtraEdit = "EDIT";

        bool IsInEditMode
        {
            get
            {
                var intent = Intent;
                var edit = false;
                if (intent != null)
                {
                    edit = intent.GetBooleanExtra(ExtraEdit, false);
                }
                return edit;
            }
        }

        public static void Start (Activity activity, bool edit, ActivityOptions options)
		{
			var starter = new Intent (activity, typeof(SignInActivity));
			starter.PutExtra (ExtraEdit, edit);
			if (options == null) {
				activity.StartActivity (starter);
				activity.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,
					Android.Resource.Animation.SlideOutRight);
			} else {
				activity.StartActivity (starter, options.ToBundle ());
			}
		}

		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_sign_in);
			var edit = IsInEditMode;
			if (savedInstanceState == null) {
				FragmentManager.BeginTransaction ().Replace (Resource.Id.sign_in_container, SignInFragment.Create (edit)).Commit ();
			}
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			if (PreferencesHelper.IsSignedIn (this)) {
				Finish ();
			}
		}
	}
}

