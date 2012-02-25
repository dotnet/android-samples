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

namespace RotationDemo
{
	[Activity (Label = "SimpleStateActivity")]
	public class SimpleStateActivity : Activity
	{
		int c;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.SetContentView (Resource.Layout.SimpleStateView);
			
			var output = this.FindViewById<TextView> (Resource.Id.outputText);
			
			if (bundle != null)
				c = bundle.GetInt ("counter", 0);
			else
				c = 0;
			
			output.Text = c.ToString ();

			var incrementCounter = this.FindViewById<Button> (Resource.Id.incrementCounter);
			
			incrementCounter.Click += (s,e) => { 
				output.Text = (++c).ToString();
			};
		}
		
		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt ("counter", c);
			base.OnSaveInstanceState (outState);
		}
	}
}

