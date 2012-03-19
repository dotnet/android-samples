using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Android.Util;

namespace LocalFiles
{
	[Activity (Label = "Local Files sample", MainLauncher = true)]
	public class Activity1 : Activity
	{
		int count = 0;
		static readonly string Filename = "count";
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			string path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			string filename = Path.Combine (path, Filename);
			if (File.Exists (filename)) {
				using (var f = new StreamReader (OpenFileInput (Filename))) {
					string line;
					do {
						line = f.ReadLine ();
					} while (!f.EndOfStream);
					int.TryParse (line, out count);
				}
			}

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			Button btnSave = FindViewById<Button> (Resource.Id.btnSave);
			Button btnReset = FindViewById<Button> (Resource.Id.btnReset);
			TextView txtStored = FindViewById<TextView> (Resource.Id.stored);
			TextView txtPath = FindViewById<TextView> (Resource.Id.path);

			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", ++count); };

			btnSave.Click += delegate {
				using (var f = new StreamWriter (OpenFileOutput (Filename, FileCreationMode.Append | FileCreationMode.WorldReadable))) {
					f.WriteLine (count);
				}
				btnSave.Text = string.Format ("Current count saved: {0}", count);
				txtStored.Text = string.Format (this.GetString (Resource.String.stored), count);
			};

			btnReset.Click += delegate {
				File.Delete (filename);
				btnSave.Text = this.GetString (Resource.String.save);
			};
			txtPath.Text = filename;
			txtStored.Text = string.Format (this.GetString (Resource.String.stored), count);
		}
	}
}
