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

using System.Threading.Tasks;
using System.Threading;

namespace LocalFiles
{
	[Activity (Label = "Local Files sample", MainLauncher = true)]
	public class Activity1 : Activity
	{
		int count = 0;
		static readonly string Filename = "count";
		string path;
		string filename;


		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			filename = Path.Combine (path, Filename);

			Task<int> loadCount = loadFileAsync ();

			Console.WriteLine ("Could be excueted before load finished!");

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			Button btnSave = FindViewById<Button> (Resource.Id.btnSave);
			Button btnReset = FindViewById<Button> (Resource.Id.btnReset);
			TextView txtStored = FindViewById<TextView> (Resource.Id.stored);
			TextView txtPath = FindViewById<TextView> (Resource.Id.path);

			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", ++count); };

			btnSave.Click += async delegate {

				btnSave.Text = string.Format ("Current count saved: {0}", count);
				txtStored.Text = string.Format (this.GetString (Resource.String.stored), count);
				await writeFileAsync();
			};

			btnReset.Click += delegate {
				File.Delete (filename);
				btnSave.Text = this.GetString (Resource.String.save);
				txtStored.Text = string.Format (this.GetString (Resource.String.stored), 0);
			};

			count = await loadCount;
			txtPath.Text = filename;
			txtStored.Text = string.Format (this.GetString (Resource.String.stored), count);
		}

		async Task<int> loadFileAsync()
		{

			if (File.Exists (filename)) {
				using (var f = new StreamReader (OpenFileInput (Filename))) {
					string line;
					do {
						line =await f.ReadLineAsync();
					} while (!f.EndOfStream);
					Console.WriteLine ("Load Finished");
					return int.Parse (line);

				}
			}
			return 0;
		}

		async Task writeFileAsync()
		{
			using (var f = new StreamWriter (OpenFileOutput (Filename, FileCreationMode.Append | FileCreationMode.WorldReadable))) {
				await f.WriteLineAsync (count.ToString ()).ConfigureAwait(false);
			}
			Console.WriteLine ("Save Finished!");

		}
	}
}
