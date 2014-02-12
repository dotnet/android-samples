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
using Android.Print;

namespace KitKat
{
	[Activity (Label = "PrintersActivity")]			
	public class PrintersActivity : Activity
	{

		Button imagePrintButton;
		Button webPrintButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Printers);

			imagePrintButton = FindViewById<Button> (Resource.Id.imagePrintButton);
			webPrintButton = FindViewById<Button> (Resource.Id.webPrintButton);

			webPrintButton.Click += (o, e) => {
				StartActivity (typeof(PrintHtmlActivity));
			};

			imagePrintButton.Click += (o, e) => {
				// To be added when support framework gets updated
			};

		}
	}
}

