
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace AutofillFramework.app
{
	[Activity(Label = "EmailComposeActivity")]
	public class EmailComposeActivity : AppCompatActivity
	{
		public static Intent GetStartActivityIntent(Context context)
		{
			Intent intent = new Intent(context, typeof(EmailComposeActivity));
        	return intent;
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.email_compose_activity);
			FindViewById(Resource.Id.sendButton).Click += (sender, args) => {
				StartActivity(WelcomeActivity.GetStartActivityIntent(this));
				Finish();
			};
		}
	}
}
