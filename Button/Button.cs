using System;

using Android.App;
using Android.OS;
using Android.Runtime;

namespace Mono.Samples.Button
{
	[Activity (Label = "Button Demo", MainLauncher = true)]
	public class ButtonActivity : Activity
	{
		int count = 0;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			Android.Widget.Button button = new Android.Widget.Button (this);

			button.Text = string.Format ("{0} clicks!!", count);
			button.Click += delegate { button.Text = string.Format ("{0} clicks!!", ++count); };

			SetContentView (button);
		}
	}
}

